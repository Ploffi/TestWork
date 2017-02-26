using System.Collections.Generic;
using LiteDB;
using WebApi.Data;

namespace WebApi.Services
{
    internal class MatchPlayerContractService
    {
        public MatchPlayerContractService()
        {

        }

        public void Insert(IEnumerable<MatchPlayerContract> mpContracts)
        {
            using (var db = new LiteDatabase(Config.DbPath))
            {
                db.GetCollection<MatchPlayerContract>(Config.MPContract)
                    .Insert(mpContracts);
            }
        }
    }
}