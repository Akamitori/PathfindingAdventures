namespace ClassLibrary1;

public class Entrance {
    private readonly int length;
    private readonly HiearchicalGraph.Adjacency adjacency;

    public Coords[] tiles { get; private set; }
    public Coords[] symmetricalTiles { get; private set; }


    public Entrance(int length, Coords[] tiles, Coords[] symmetricalTiles) {
        this.length = length;
        this.tiles = tiles;
        this.symmetricalTiles = symmetricalTiles;
    }
}