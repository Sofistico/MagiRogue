using MagusEngine.Core.MapStuff;
using MagusEngine.Generators.MapGen;
using MagusEngine.Services;
using SadRogue.Primitives;

namespace MagusEngine.Systems
{
    // Under construction...
    public class ChunkService
    {
        private readonly SavingService _savingService;
        private readonly IDGenerator _idGenerator;
        private readonly MessageBusService _messageBus;

        public ChunkService()
        {
            _messageBus = Locator.GetService<MessageBusService>();
            _messageBus.RegisterAllSubscriber(this);
            _savingService = Locator.GetService<SavingService>();
            _idGenerator = Locator.GetService<IDGenerator>();
        }

        public RegionChunk GenerateChunck(Point posGenerated)
        {
            RegionChunk newChunck = new(posGenerated);

            WildernessGenerator genMap = new();
            newChunck.LocalMaps = genMap.GenerateMapWithWorldParam(Find.Universe.WorldMap, posGenerated);

            for (int i = 0; i < newChunck.LocalMaps.Length; i++)
            {
                newChunck.LocalMaps[i].SetId(_idGenerator.UseID());
            }

            return newChunck;
        }

        public RegionChunk? GetChunckByPos(Point playerPoint)
        {
            return _savingService.GetChunkAtIndex(playerPoint, Find.Universe.PlanetSettings!.PlanetWidth);
        }
    }
}
