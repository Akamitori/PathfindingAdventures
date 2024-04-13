using ClassLibrary1;
using ClassLibrary1.Graph;
using ClassLibrary1.GraphBuilder;

namespace Runner;

public class Poc {
    public static void Run(int[,] someMap) {
        var graphBuilder = new GraphBuilderFromMapWithDiagonals(someMap);
        var x = graphBuilder.BuildGraph();

        int ManhattanDistanceHeuristic((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);


        var algo = new AStar(x, 0, 0, 2, 1, ManhattanDistanceHeuristic, _ => true);

        while (!algo.ExecuteStep()) {
            ;
        }

        var result = algo.GetPath();

        for (var i = 0; i < result.GetLength(0); i++) {
            var currentNode = result[i];
            Display(x, currentNode);
            Thread.Sleep(1000);
            if (i != result.GetLength(0) - 1) {
                Console.Clear();
            }
        }

        var totalCost = algo.GetPathCost();

        var s = "[";
        for (var i = 0; i < result.GetLength(0); i++) {
            s += $"{result[i]}, ";
        }

        s = s.Trim(' ');
        s = s.Trim(',');
        s += "]";

        Console.WriteLine(s);
        Console.WriteLine(totalCost);

        return;

        void Display(Graph graph, int currentNode) {
            var (row, column) = graph.ConvertFromId(currentNode);

            for (var i = 0; i < someMap.GetLength(0); i++) {
                var s = "";
                for (var j = 0; j < someMap.GetLength(1); j++) {
                    char thingToDisplay;
                    if (i == row && j == column) {
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