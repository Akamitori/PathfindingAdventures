namespace ClassLibrary1.Graph;

public abstract class Graph<T> {
    public Node<T>[] Nodes;

    public EdgeInfo[,] Adjacency;

    //public int[,] Map;

    public int GraphId;

    public Graph(Node<T>[] nodes, EdgeInfo[,] adjacency) {
        Nodes = nodes;
        Adjacency = adjacency;
    }

    public abstract int ConvertToId(T nodeInfo);
    
    public abstract T ConvertFromId(int id);
    
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