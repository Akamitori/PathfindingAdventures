using ClassLibrary1.GraphBuilder;

namespace ClassLibrary1.HierachicalGraph;

public class Entrance {
    private readonly int length;
    private readonly RelativePosition relativePosition;

    public Coords[] tiles { get; private set; }
    public Coords[] symmetricalTiles { get; private set; }
    
    public Entrance(int length, Coords[] tiles, Coords[] symmetricalTiles) {
        this.length = length;
        this.tiles = tiles;
        this.symmetricalTiles = symmetricalTiles;
    }
}