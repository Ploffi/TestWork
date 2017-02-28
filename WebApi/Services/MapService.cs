using WebApi.Data;
using WebApi.Repository;

namespace WebApi.Services
{
    internal class MapService
    {
        private MapRepository _mapRepository;
        public MapService()
        {
            _mapRepository = new MapRepository();
        }

        public Map GetOrInsertByName(string mapName)
        {
            var map = _mapRepository.GetByName(mapName);
            if (map != null) return map;
            map = new Map() {Name = mapName};
            _mapRepository.Insert(map);
            return map;
        }
    }
}