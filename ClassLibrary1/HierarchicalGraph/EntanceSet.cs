using ClassLibrary1.GraphBuilder;
using ClassLibrary1.HierarchicalGraph;

namespace ClassLibrary1.HierachicalGraph;

public class EntranceSet {
    private readonly RelativePosition relativePosition;

    public Entrance Cluster1Entrance;

    public Entrance Cluster2Entrance;

    public readonly int length;

    public EntranceSet(Coords[] tiles, int tileOwner, RelativePosition entranceRelativePosition,
        Coords[] symmetricalTiles, int symmetricalTileOwner, RelativePosition symmetricalEntranceRelativePosition) {
        Cluster1Entrance = new Entrance(tileOwner, tiles, entranceRelativePosition);
        Cluster2Entrance = new Entrance(symmetricalTileOwner, symmetricalTiles, symmetricalEntranceRelativePosition);
        length = tiles.Length;
    }
}