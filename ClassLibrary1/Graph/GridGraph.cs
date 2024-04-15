using ClassLibrary1.HierachicalGraph;

namespace ClassLibrary1.Graph;

public class GridGraph : Graph<Coords> {
    private readonly int gridColumns;

    public GridGraph(Node<Coords>[] nodes, EdgeInfo[,] adjacency, int mapColumns) : base(nodes, adjacency) {
        gridColumns = mapColumns;
    }
    
    public override int ConvertToId(Coords nodeInfo) {
        return nodeInfo.Y * gridColumns + nodeInfo.X;
    }

    public override Coords ConvertFromId(int id) {
        return Nodes[id].Data;
        // var row = id / gridColumns;
        // var column = id % gridColumns;
        // return new Coords(row, column);
    }
}