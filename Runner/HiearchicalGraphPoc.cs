using ClassLibrary1;
using ClassLibrary1.Graph;
using ClassLibrary1.GraphBuilder;
using ClassLibrary1.HierachicalGraph;
using ClassLibrary1.HierarchicalGraph;

namespace Runner;

public class HiearchicalGraphPoc {
    public static void Run(int[,] someMap, int clusterSize) {
        var x = new HierarchicalGraphBuilder(someMap, clusterSize);
        var g = x.BuildGraph();


        int ManhattanDistanceHeuristic(ClusterNodeInfo a, ClusterNodeInfo b) =>
            Math.Abs(a.NodeCoords.X - b.NodeCoords.X) + Math.Abs(a.NodeCoords.Y - b.NodeCoords.Y);

        // // from (5,3) to (0,2)
        var a = new AStar<ClusterNodeInfo>(g,
            new ClusterNodeInfo(new Coords(3, 5), -1),
            new ClusterNodeInfo(new Coords(2, 0), -1),
            (_,_)=>0,
            _ => true
        );

        var found = a.ExecuteToCompletion(out var result, out var cost);

        for (var i = 0; i < result.GetLength(0); i++) {
            var currentNode = result[i];
            Display(g, currentNode);
            Thread.Sleep(1000);
            if (i != result.GetLength(0) - 1) {
                Console.Clear();
            }
        }

        var totalCost = cost;

        var s = "[";
        for (var i = 0; i < result.GetLength(0); i++) {
            var nodeInfo = g.ConvertFromId(result[i]);
            s += $"{result[i]}, ";
        }

        s = s.Trim(' ');
        s = s.Trim(',');
        s += "]";

        Console.WriteLine(s);
        Console.WriteLine(totalCost);

        return;

        void Display(Graph<ClusterNodeInfo> graph, int currentNode) {
            var result = graph.ConvertFromId(currentNode);

            for (var i = 0; i < someMap.GetLength(0); i++) {
                var s = "";
                for (var j = 0; j < someMap.GetLength(1); j++) {
                    char thingToDisplay;
                    if (i == result.NodeCoords.Y && j == result.NodeCoords.X) {
                        thingToDisplay = 'X';
                    }
                    else {
                        thingToDisplay = someMap[i, j] >= 0 ? ' ' : 'O';
                    }

                    s += $"|{thingToDisplay}|";
                }

                Console.WriteLine(s);
            }
        }
    }
}