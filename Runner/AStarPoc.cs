using ClassLibrary1;
using ClassLibrary1.Graph;
using ClassLibrary1.GraphBuilder;
using ClassLibrary1.HierachicalGraph;

namespace Runner;

public class AStarPoc {
    public static void Run(int[,] someMap) {
        var graphBuilder = new GraphBuilderFromMapWithDiagonals(someMap);
        var x = graphBuilder.BuildGraph();

        int ManhattanDistanceHeuristic(Coords a, Coords b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
        var algo = new AStar<Coords>(x, new Coords(0, 0), new Coords(2, 1), ManhattanDistanceHeuristic, _ => true);

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

        void Display(Graph<Coords> graph, int currentNode) {
            var result = graph.ConvertFromId(currentNode);

            for (var i = 0; i < someMap.GetLength(0); i++) {
                var s = "";
                for (var j = 0; j < someMap.GetLength(1); j++) {
                    char thingToDisplay;
                    if (i == result.X && j == result.Y) {
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