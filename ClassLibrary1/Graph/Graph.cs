namespace ClassLibrary1.Graph;

public abstract class Graph {
    public Node[] Nodes;

    public EdgeInfo[,] Adjacency;

    //public int[,] Map;

    public int GraphId;

    public Graph(Node[] nodes, EdgeInfo[,] adjacency) {
        Nodes = nodes;
        Adjacency = adjacency;
    }

    public abstract int ConvertToId(int x, int y);
    
    public abstract (int, int) ConvertFromId(int id);
    
    public List<EdgeInfo> GetNeighborsOfNode(int nodeId) {
        var result = new List<EdgeInfo>();
        for (var i = 0; i < Adjacency.GetLength(1); i++) {
            var edge = Adjacency[nodeId, i];

            if (edge.enabled) {
                result.Add(edge);
            }
        }

        return result;
    }
}