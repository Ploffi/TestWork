# Architecture Review — Kontur.GameStats.Server

Reviewed revision: `16b2c5a` (`master`).
Scope: the whole repository: `WebApi` (the service), `Tests`, `WebApi.Tests`, the solution and configuration files.
Method: I read the source by hand. No build or runtime checks were possible because the review
environment has no .NET Framework / Mono toolchain. Anything that depends on how a library behaves
at runtime is marked **(verify)**.

---

## 1. System overview

A self-hosted ASP.NET Web API 2 (OWIN-less `HttpSelfHostServer`, .NET Framework 4.5.2) that
collects game-server statistics and serves reports. Storage is an embedded LiteDB 3.0.1 file
(`data.db`). A second LiteDB file (`utils.db`) holds a single "max match date" value.

```
HTTP ─► Controllers ─► Services ─► Repositories ─► LiteDB (data.db, utils.db)
            │                          ▲
            └──────────────────────────┘   (controllers also call repositories directly)
Responses: Data entities ─► global Newtonsoft JsonConverters ─► JSON
```

| Layer        | Location                    | Notes |
|--------------|-----------------------------|-------|
| Host / bootstrap | `WebApi/Program.cs`, `Config.cs`, `App.config` | Builds the URL from argv, registers converters and the NLog trace writer |
| Controllers  | `WebApi/Controllers/*`      | `api/servers`, `api/players`, `api/reports` |
| Services     | `WebApi/Services/*`         | Aggregation logic (stats, popularity), ingest orchestration |
| Repositories | `WebApi/Repository/*`       | One `LiteDatabase` instance per method call |
| Entities     | `WebApi/Data/*`             | LiteDB documents with `[BsonRef]` relations |
| DTOs         | `WebApi/Models/*`           | Input models plus output stats; one entity (`DateInfo`) also lives here |
| Serialization| `WebApi/JsonConvertors/*`   | Hand-written converters that write entities straight to the wire |

The layering idea is reasonable for a small service. The main risks are in **data consistency,
concurrency and correctness of the report queries**, not in the overall shape.

---

## 2. Findings by severity

### 🔴 Critical: correctness and data integrity

**C1. Match ingest is not atomic and not idempotent.**
`MatchService.TryInsert` (`WebApi/Services/MatchService.cs:29-60`) does six separate writes, each
opening its own `LiteDatabase`:
`UtilRepository.UpdateMaxDate`, then `MapRepository.Insert`, `MatchRepository.Insert`,
`PlayerRepository.GetOrInsertByScoreModel` (which runs its own transaction), `ScoreRepository.Insert`,
and finally `MatchRepository.Update`.
A failure partway through leaves orphan matches without a scoreboard, or player totals that are
already incremented for a match that was never fully stored. Sending the same
`PUT /servers/{ep}/matches/{ts}` again inserts a second match and **counts the player's kills, deaths
and matches twice**, because nothing checks for an existing `(ServerId, Date)`.
*Recommendation:* put the whole ingest in one `LiteDatabase` + `BeginTrans()`. Add a unique
`(ServerId, Date)` key, or check for it first, so repeated PUTs are idempotent upserts.

**C2. Concurrent requests share one database file with no coordination, and journaling is off on some writes.**
Web API self-host serves requests in parallel. Every repository call opens `data.db` again
(`new LiteDatabase(Config.DbPath)`). `MapRepository.Insert` uses `Config.JournalOff`
(`journal=false`, `WebApi/Repository/MapRepository.cs:19`), so a crash or a concurrent writer can
corrupt the file. Parallel ingests also race on the "get or insert" paths for maps, game modes and
players. The unique indexes turn those races into unhandled exceptions, which reach the client as 500s.
*Recommendation:* use one long-lived `LiteDatabase` (singleton, thread-safe in LiteDB) or a single
writer queue. Never turn the journal off for the primary store.

**C3. Lazy query results are returned from inside `using (var db = ...)` (verify).**
Most read methods return `collection.Find(...)` / `FindAll()` straight out of a `using` block, for
example `MatchRepository.GetRecentMatches` (`WebApi/Repository/MatchRepository.cs:61-73`),
`PlayerRepository.GetBest` and `ServerRepository.GetAll`. In LiteDB 3.x these are lazily enumerated
`IEnumerable<T>`. The caller calls `.ToList()` only *after* the database has been disposed. Whether
this works depends on library internals, and it can throw `ObjectDisposedException` or read from a
closed stream.
*Recommendation:* materialize (`.ToList()`) inside the `using`. Better, use the shared database
instance from C2.

**C4. Report endpoints return wrong results.**

| Endpoint | Location | Defect |
|---|---|---|
| `GET /reports/best-players/{n}` | `ReportsController.cs:56-59`, `PlayerRepository.cs:63-72` | The DB query takes the **first `n`** qualifying players in index scan order, and only those `n` are sorted by K/D afterwards. The result is not the top `n`. The filters `GT(TotalMatchesPlayed, 10)` and `GT(TotalDeaths, 0)` are strict (`>`). Confirm the spec does not mean `>= 10`. |
| `GET /reports/popular-servers/{n}` | `ServerService.cs:114-137` | `serverMinDateDict` is updated with `.Max(...)`, so it tracks each server's **latest** date, not its earliest. That inflates average matches per day. The result is `serversDict.Values.Take(count)` with **no ordering** by popularity. |
| `GET /servers/{ep}/matches/{ts}` | `MatchRepository.cs:28-46` | Looks up the match by `Date` only, then compares the server. If two servers report a match with the same timestamp, `FindOne` may pick the other server's match, and the request returns 404 for a match that exists. Query by `ServerId` and `Date` together. |
| `GET /players/{name}/stats` | `PlayerController.cs:24-26` | An unknown player returns **400** instead of **404**. |

**C5. Timestamps are not handled consistently with respect to time zone and culture.**
`PutMatches` uses `DateTime.Parse(timestamp)` (`ServerController.cs:46`), which depends on the
current culture and throws on bad input, giving a 500. `GetMatchInfo` uses `TryParse`, so the two
endpoints behave differently. `ToIsoFormat` (`Services/Extensions.cs:54`) adds a literal `Z` without
converting to UTC, so values with `Kind=Local` are serialized wrongly. LiteDB also stores `DateTime`
in its own way (verify the round-trip `Kind`).
*Recommendation:* parse with `DateTimeOffset.TryParse(..., CultureInfo.InvariantCulture,
DateTimeStyles.AdjustToUniversal)`, store UTC, and format with `"o"` or an explicit UTC conversion.

### 🟠 Major: architecture and maintainability

**M1. No dependency injection; dependencies are created with `new` in constructors.**
Every controller, service and repository creates its own collaborators. `MatchService` builds a
6-object graph, and `ScoreService` creates a `PlayerService` it never uses (`ScoreService.cs:17`).
Because of this, nothing can be unit-tested without the real `data.db`. That is probably why the
test projects are dead (M5).
*Recommendation:* add interfaces (`IMatchRepository`, …), use constructor injection, and register
them through an `IDependencyResolver` (Unity, Autofac, or a small hand-rolled resolver).

**M2. Layers leak into each other.**
- `ReportsController.GetRecentMatches` (`ReportsController.cs:33-44`) talks to two repositories and
  joins players in memory. That is service logic placed in a controller.
- `MatchModel` (an input DTO) carries a `Data.Server` entity (`Models/MatchModel.cs:18`). The
  controller fills it before passing it to the service.
- `DateInfo`, a persisted entity, lives in `Models`. `Repository/UtilRepository.cs` imports
  `WebApi.Services` just to use an extension method.
- `Services/Extensions.cs` mixes dictionary helpers, date helpers and a `JsonWriter` helper that the
  serialization layer uses.

**M3. Entities double as the wire contract.**
Data entities are serialized directly by six converters registered **globally** for whole types
(`Program.cs:34-39`). As a result:
- Any change to the storage model can silently change the API, and the reverse is also true.
- Converters write numbers as **strings** (`fragLimit`, `kills`, `killToDeathRatio`, etc. via
  `WriteProperty(string, string)`). Clients may expect JSON numbers. Check this against the spec.
- `RecentMatchesConvertor` / `ServerListConvertor` match on `List<T>` exactly. Returning an array or
  an `IEnumerable` from a controller silently skips them.
- `PlayerConverter` and `ServerListConvertor` have no `null` guard.
*Recommendation:* add explicit response DTOs per endpoint and map entities to them in the service
layer. Then the custom converters can go away.

**M4. Every report is computed on the fly with full scans.**
`ServerService.GetPopular` loads **all matches and all servers** on every request
(`ServerService.cs:98-102`). `GetServerStats` loads every match of a server with four `Include`s, and
player stats load every score and match of the player. Response time grows linearly with history.
*Recommendation:* keep per-server, per-player and per-day aggregates up to date at ingest time,
inside the same transaction as C1. Reports then read precomputed rows. You may also cache the
`reports/*` responses for a short time.

**M5. Build and test setup is broken.**
- `Kontur.GameStats.Server.sln` contains only `WebApi`. `Tests` and `WebApi.Tests` are not in the solution.
- `Tests/ServerService.cs:45` calls `AdvertiseServer(string, string)`, but the action signature is
  `AdvertiseServer(ServerModel, string)`, so the file does not compile. It also compares a `Server`
  with an `OkNegotiatedContentResult<Server>` (`:49`), and the class has no `[TestFixture]`.
- `WebApi.Tests.csproj` references `Controllers/HomeControllerTest.cs`, `ValuesControllerTest.cs`
  and `..\WebApplication1\WebApplication1.csproj`, none of which exist. It is template leftover.
- `WebApi/Models/Server/ServerInfo.cs` is not in `WebApi.csproj`, so it is dead code. It also
  duplicates `Models/ServerInfo.cs`.
*Recommendation:* delete `WebApi.Tests`, fix `Tests` and add it to the solution, and add CI that at
least builds the solution.

### 🟡 Minor: robustness and hygiene

- **Input validation gaps:** if the request body is missing, `model` is `null` and
  `model.IsNotValid()` throws a NullReferenceException. `MatchModel.IsNotValid` dereferences
  `ScoreBoard` without a null check (`MatchModel.cs:38`). There is no global `ExceptionFilter` or
  `IExceptionHandler`, so all of these become unlogged 500s.
- **`NLogger.Trace` condition is always true:** `level != Off || level != Info`
  (`Utils/NLogger.cs:31`). If `TraceLevel.Off` is ever passed, `Logger[record.Level]` throws
  `KeyNotFoundException`. The condition should be `&&`.
- **Fragile startup:** `Program.Main` takes `args.Skip(1).First()`, splits it on `+`, and rebuilds
  the URL as `prefix + "localhost" + port`, which ignores the host the caller passed
  (`Program.cs:15-19`). It throws on missing args with no usage message. `Console.ReadKey` keeps the
  process alive, so it cannot run as a service or in a container without a TTY.
- **Configuration:** connection strings are built by concatenating strings (`Config.cs:20-25`).
  `CasheSize` in `App.config:15` looks like a typo and is probably ignored (verify against LiteDB 3
  connection-string keys). `MPContract` is unused. The `system.web` membership and role-provider
  sections are template boilerplate.
- **`utils.db` is unnecessary:** the global max date can be read from the indexed `Match.Date`
  (`Query.All("Date", Query.Descending)`, limit 1). That removes a second file and one more
  non-atomic write. `GetMaxDate` also calls `.First()`, which throws on an empty collection.
- **`Server.Equals` without `GetHashCode`:** `Data/Server.cs:18-24` overrides equality without a
  hash code, and it compares `GameModes` by reference.
- **`[BsonIndex(true)]` on `Score.Player`** (`Data/Score.cs:10`) declares a *unique* index on a
  reference field. Each player has many scores, so if LiteDB ever builds this index, the second
  score for the same player fails (verify). Queries use `Player.$id`, so the index is probably never
  created. Remove the attribute either way.
- **Player names are lowercased on write** (`PlayerService.cs:33`). Reports therefore never return
  the original casing. Keep a display name separately from the normalized lookup key.
- **`ScoreService.InsertOrderedScores`** maps scores to players by a mutable index captured in a LINQ
  closure (`ScoreService.cs:22-34`). It works only because `Select` runs once, in order, before
  `ToList()`. Use `Select((model, i) => ...)` instead.
- **Naming:** `Convertor`/`Converter` spelling is mixed, `selfHostConfiguraiton` and
  `qeuryCreator` are misspelled, and `PlayerStats`/`ServerStats` use public lower-case fields to shape
  the JSON. `[JsonProperty]` or a camel-case contract resolver would be cleaner.
- **Dependencies:** .NET Framework 4.5.2, Web API 5.2.3, LiteDB 3.0.1 and Newtonsoft 9.0.1 are all
  far out of support. Consider ASP.NET Core with Kestrel and LiteDB 5 (the file format changed, so a
  migration is needed) if the service is going to be maintained.

---

## 3. Suggested roadmap

1. **Stabilize data (C1–C3):** one shared `LiteDatabase`, one transaction per ingest, idempotent
   match upsert, journaling always on, and query results materialized inside the database scope.
2. **Fix report correctness (C4, C5):** sort before `Take`, fix the min-date bug, look up matches by
   server and timestamp together, handle timestamps in UTC, and return the right status codes.
3. **Make it testable (M1, M5):** add interfaces and DI, repair the `Tests` project, add it to the
   solution, and run a build/test in CI.
4. **Decouple the contract (M2, M3):** add explicit response DTOs, move the join logic out of
   `ReportsController`, and remove the global type converters.
5. **Scale reads (M4):** keep aggregates up to date at ingest time and cache report endpoints.
6. **Modernize (optional):** move to ASP.NET Core, the generic host and LiteDB 5.

---

_Produced by Air Automations. Name: Say hello / Run: https://air.jetbrains.cloud/org/9b39cdad-f8df-741b-abd3-29d0c8a05e2d/automations/413d9d98-a3a9-46cc-ac19-3001d4a7bf27?run=305bfaa6-e66b-479d-b84a-9dcef807aa38_
