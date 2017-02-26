using WebApi.Data;
using WebApi.Repository;

namespace WebApi.Services
{
    public class GameModeService
    {
        private GameModeRepository _gameModeRepository;

        public GameModeService()
        {
            _gameModeRepository = new GameModeRepository();
        }
    }
}