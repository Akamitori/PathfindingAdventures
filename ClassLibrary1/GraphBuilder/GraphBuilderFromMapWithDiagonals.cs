using ClassLibrary1.HierarchicalGraph;

namespace ClassLibrary1.GraphBuilder;

public class GraphBuilderFromMapWithDiagonals : GraphBuilderFromMap {
    public GraphBuilderFromMapWithDiagonals(int[,] map) : base(map, 8) {
    }
    
    protected override List<(int, int)> GetNeighborIndexes(int i, int j) {
        var result = base.GetNeighborIndexes(i, j);
        var directions = new[] {
            (-1 + i, j + 1),    // upRight
            (-1 + i, j - 1),    // upLeft 
            (1 + i, j + 1),     //downRight
            (1 + i, j - 1)      //downLeft
        };

        result.AddRange(directions.Where(c => mapBounds.ValueInBounds(c)));

        return result;
    }

    protected override float
        CalculateEdgeCost(int neighborCost, (int i, int j) valueTuple, (int x, int y) valueTuple1) {
        var distance = ManhattanDistance(valueTuple, valueTuple1);

        float extraCost = 0f;
        if (distance == 1) {
            extraCost = 1;
        }
        else if (distance == 2) {
            extraCost = MathF.Sqrt(2);
        }
        else {
            throw new InvalidCastException();
        }


        return base.CalculateEdgeCost(neighborCost, valueTuple, valueTuple1) + extraCost;
    }

    public int ManhattanDistance((int x, int y) a, (int x, int y) b) => Math.Abs(a.x - b.x) + Math.Abs(a.y - b.y);
}