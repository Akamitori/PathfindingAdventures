namespace ClassLibrary1;

public class GraphBuilderFromMap<T> : IGraphBuilder {
    private readonly int[,] map;

    public GraphBuilderFromMap(int[,] map) {
        this.map = map;
    }

    public Graph BuildGraph() {
        var xLength = map.GetLength(0);
        var yLength = map.GetLength(1);
        var numberOfNodes = xLength * yLength;
        var adjacencyTable = new EdgeInfo[numberOfNodes, 4];
        var nodeCount = 0;
        var nodeSet = new Node[numberOfNodes];

        var mapRows = map.GetLength(0);
        var mapColumns = map.GetLength(1);

        for (var i = 0; i < xLength; i++) {
            for (var j = 0; j < yLength; j++) {
                var neighborIndexes = GetNeighborIndexes(i, j, mapRows, mapColumns);


                var nodeId = GraphHelperMethods.ConvertToId(i, j, map.GetLength(1));
                var node = new Node(i, j, nodeId);
                nodeSet[nodeCount] = node;

                for (var k = 0; k < neighborIndexes.Count; k++) {
                    var (x, y) = neighborIndexes[k];
                    var neighborFlag = map[x, y];
                    var neighborNodeId = GraphHelperMethods.ConvertToId(x, y, map.GetLength(1));
                    var edgeInfo = new EdgeInfo() {
                        neighbor = neighborNodeId,
                        enabled = neighborFlag >= 0,
                        cost = neighborFlag >= 0 ? neighborFlag : int.MaxValue
                    };
                    adjacencyTable[nodeId, k] = edgeInfo;
                }

                nodeCount++;
            }
        }

        return new Graph(nodeSet, adjacencyTable);
    }


    private List<(int, int)> GetNeighborIndexes(int i, int j, int mapRows, int mapColumns) {
        var up = (-1 + i, j);
        var down = (1 + i, j);
        var left = (i, -1 + j);
        var right = (i, 1 + j);


        var result = new List<(int, int)>();
        AddIfInBounds(left, result, mapRows, mapColumns);
        AddIfInBounds(right, result, mapRows, mapColumns);
        AddIfInBounds(up, result, mapRows, mapColumns);
        AddIfInBounds(down, result, mapRows, mapColumns);

        return result;
    }

    private void AddIfInBounds((int, int) itemToAdd, ICollection<(int, int)> result, int mapRows, int mapColumns) {
        if (GraphHelperMethods.InBounds(itemToAdd, mapRows, mapColumns)) {
            result.Add(itemToAdd);
        }
    }
}