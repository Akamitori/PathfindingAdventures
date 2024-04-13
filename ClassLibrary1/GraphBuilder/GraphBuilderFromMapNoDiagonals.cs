using ClassLibrary1.Graph;
using ClassLibrary1.HierarchicalGraph;

namespace ClassLibrary1.GraphBuilder;

public class GraphBuilderFromMap : IGraphBuilder {
    private readonly int[,] map;
    protected readonly int numberOfEdges;
    protected Bounds2D mapBounds;

    public GraphBuilderFromMap(int[,] map, int edgeCount = 4) {
        this.map = map;
        numberOfEdges = edgeCount;
        mapBounds = new Bounds2D(map.GetLength(0), map.GetLength(1));
    }

    public Graph.Graph BuildGraph() {
        var numberOfNodes = mapBounds.FirstDimensionEnd * mapBounds.SecondDimensionEnd;
        var adjacencyTable = new EdgeInfo[numberOfNodes, numberOfEdges];
        var nodeCount = 0;
        var nodeSet = new Node[numberOfNodes];


        for (var i = 0; i < mapBounds.FirstDimensionEnd; i++) {
            for (var j = 0; j < mapBounds.SecondDimensionEnd; j++) {
                var neighborIndexes = GetNeighborIndexes(i, j);


                var nodeId = ConvertToId(i, j, mapBounds.SecondDimensionEnd);
                var node = new Node(i, j, nodeId);
                nodeSet[nodeCount] = node;

                for (var k = 0; k < neighborIndexes.Count; k++) {
                    var (x, y) = neighborIndexes[k];
                    var neighborCost = map[x, y];
                    var neighborNodeId = ConvertToId(x, y, mapBounds.SecondDimensionEnd);
                    var edgeInfo = new EdgeInfo() {
                        neighbor = neighborNodeId,
                        enabled = neighborCost >= 0,
                        cost = CalculateEdgeCost(neighborCost, (i, j), (x, y))
                    };
                    adjacencyTable[nodeId, k] = edgeInfo;
                }

                nodeCount++;
            }
        }

        return new GridGraph(nodeSet, adjacencyTable, map.GetLength(1));
    }


    protected virtual List<(int, int)> GetNeighborIndexes(int i, int j) {
        var directions = new[] {
            (-1 + i, j), // up
            (1 + i, j), //down
            (i, -1 + j), //left

            (i, 1 + j) //right
        };

        return directions.Where(c => mapBounds.ValueInBounds(c)).ToList();
    }

    protected virtual float CalculateEdgeCost(int neighborCost, (int i, int j) valueTuple, (int x, int y) valueTuple1) {
        return neighborCost >= 0 ? neighborCost : int.MaxValue;
    }
    
    private static int ConvertToId(int x, int y, int gridColumns) {
        return x * gridColumns + y;
    }
}