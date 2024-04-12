using ClassLibrary1;

public class Cluster {
    public int id;
    public Coords[,] subMap;

    public int StartY;
    public int EndY;

    public int StartX;
    public int EndX;

    public Cluster(int id, int rows, int columns, int startX, int endX, int startY, int endY) {
        this.id = id;
        StartX = startX;
        EndX = endX;
        StartY = startY;
        EndY = endY;
        subMap = new Coords[rows, columns];
    }

    public void AddItem(int y, int x, int index) {
        var columns = subMap.GetLength(1);
        var row = index / columns;
        var column = index % columns;
        subMap[row, column] = new Coords(x, y);
    }
    
    public bool Contains(Coords coords) {
        return coords.X >= StartX && coords.X <= EndX
                              && coords.Y >= StartY && coords.Y <= EndY;
    }
}