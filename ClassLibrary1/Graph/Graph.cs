using static System.Int32;

namespace ClassLibrary1;

public class Graph {
    public Node[] Nodes;

    public EdgeInfo[,] Adjacency;

    public int[,] Map;

    public int GraphId;

    public Graph(int[,] map, int id = 0) {
        this.GraphId = id;
        this.Map = map;
        lala(map, out Adjacency, out Nodes);
    }

    public Graph(Node[] nodes, EdgeInfo[,] adjacency) {
        this.Nodes = nodes;
        this.Adjacency = adjacency;
    }

    private void lala(int[,] map, out EdgeInfo[,] AdjacencyTable, out Node[] nodeSet) {
        var xLength = map.GetLength(0);
        var yLength = map.GetLength(1);
        var numberOfNodes = xLength * yLength;
        AdjacencyTable = new EdgeInfo[numberOfNodes, 4];
        var nodeCount = 0;
        nodeSet = new Node[numberOfNodes];
        for (var i = 0; i < xLength; i++) {
            for (var j = 0; j < yLength; j++) {
                var neighborIndexes = GetNeighborIndexes(i, j);


                var nodeId = GraphHelperMethods.ConvertToId(i, j, map.GetLength(1));
                var node = new Node(i, j, nodeId);
                Nodes[nodeCount] = node;

                for (var k = 0; k < neighborIndexes.Count; k++) {
                    var (x, y) = neighborIndexes[k];
                    var neighborFlag = map[x, y];
                    var neighborNodeId = GraphHelperMethods.ConvertToId(x, y, map.GetLength(1));
                    var edgeInfo = new EdgeInfo() {
                        neighbor = neighborNodeId,
                        enabled = neighborFlag >= 0,
                        cost = neighborFlag >= 0 ? neighborFlag : MaxValue
                    };
                    Adjacency[nodeId, k] = edgeInfo;
                }

                nodeCount++;
            }
        }
    }

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


    private List<(int, int)> GetNeighborIndexes(int i, int j) {
        var up = (-1 + i, j);
        var down = (1 + i, j);
        var left = (i, -1 + j);
        var right = (i, 1 + j);


        var result = new List<(int, int)>();
        AddIfInBounds(left, result);
        AddIfInBounds(right, result);
        AddIfInBounds(up, result);
        AddIfInBounds(down, result);

        return result;
    }

    private void AddIfInBounds((int, int) itemToAdd, ICollection<(int, int)> result) {
        if (InBounds(itemToAdd)) {
            result.Add(itemToAdd);
        }
    }

    private bool InBounds(int x, int y) {
        return x >= 0 && x < Map.GetLength(0)
                      && y >= 0 && y < Map.GetLength(1);
    }

    private bool InBounds((int x, int y) item) {
        return InBounds(item.x, item.y);
    }
}