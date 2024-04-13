namespace ClassLibrary1.Graph;

public class GridGraph : Graph {

    private readonly int gridColumns;
    public GridGraph(Node[] nodes, EdgeInfo[,] adjacency, int mapColumns) : base(nodes, adjacency) {
        gridColumns = mapColumns;
    }
    
    public override int ConvertToId(int x, int y) {
        return x * gridColumns + y;
    }

    public override (int, int) ConvertFromId(int id) {
        var row = id / gridColumns;
        var column = id % gridColumns;
        return (row, column);
    }
}