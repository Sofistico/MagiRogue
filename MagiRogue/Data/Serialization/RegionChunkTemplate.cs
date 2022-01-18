using MagiRogue.System;
using Newtonsoft.Json;

public class RegionChunkTemplate
{
    public int X { get; }
    public int Y { get; }

    [JsonIgnore]
    public MapTemplate[] LocalMaps { get; set; }
    public uint[] LocalMapsIds { get; set; }

    public RegionChunkTemplate(int x, int y, MapTemplate[] localMaps)
    {
        X = x;
        Y = y;
        LocalMaps = localMaps;
        LocalMapsIds = new uint[LocalMaps.Length];
        for (int i = 0; i < LocalMaps.Length; i++)
        {
            LocalMapsIds[i] = LocalMaps[i].MapId;
        }
    }

    public Map[] ReturnAsMap()
    {
        var map = new Map[LocalMaps.Length];
        for (int i = 0; i < LocalMaps.Length; i++)
        {
            map[i] = LocalMaps[i];
        }

        return map;
    }

    public static implicit operator RegionChunkTemplate(RegionChunk region)
    {
        MapTemplate[] maps = new MapTemplate[region.LocalMaps.Length];
        for (int i = 0; i < region.LocalMaps.Length; i++)
        {
            maps[i] = region.LocalMaps[i];
        }
        return new(region.X, region.Y, maps);
    }

    public static implicit operator RegionChunk(RegionChunkTemplate template)
    {
        var regio = new RegionChunk(template.X, template.Y)
        {
            LocalMaps = template.ReturnAsMap()
        };
        return regio;
    }
}