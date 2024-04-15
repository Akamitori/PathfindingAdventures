using ClassLibrary1.GraphBuilder;
using ClassLibrary1.HierachicalGraph;

namespace ClassLibrary1.HierarchicalGraph;

public class Entrance {
    private readonly RelativePosition positionInCluster;
    public Coords[] Tiles { get; private set; }

    public int TileOwner { get; private set; }
    
    

    public Entrance(int tileOwner, Coords[] tiles, RelativePosition positionInCluster) {
        this.positionInCluster = positionInCluster;
        TileOwner = tileOwner;
        Tiles = tiles;
    }
}