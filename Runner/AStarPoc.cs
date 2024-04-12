using ClassLibrary1;

namespace Runner;

public class Poc {
    public static void Run(int[,] someMap) {
        var x = new Graph(someMap);

        int ManhattanDistanceHeuristic((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);


        var algo = new AStar(x, 0, 0, 2, 1, ManhattanDistanceHeuristic);

        while (!algo.ExecuteStep()) {
            ;
        }

        var result = algo.GetPath();

        var columns = someMap.GetLength(1);
        for (var i = 0; i < result.GetLength(0); i++) {
            var currentNode = result[i];
            Display(currentNode);
            Thread.Sleep(1000);
            Console.Clear();
        }

        return;

        void Display(int currentNode) {
            var (row, column) = GraphHelperMethods.ConvertFromId(currentNode, x.Map.GetLength(1));

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