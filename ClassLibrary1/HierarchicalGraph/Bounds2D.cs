namespace ClassLibrary1.HierarchicalGraph;

public struct Bounds2D {
    public int FirstDimensionStart { get; private set; }
    public int FirstDimensionEnd { get; private set; }
    public int SecondDimensionStart { get; private set; }
    public int SecondDimensionEnd { get; private set; }

    public Bounds2D(int firstDimensionStart, int firstDimensionEnd, int secondDimensionStart, int secondDimensionEnd) {
        SecondDimensionStart = 0;
        FirstDimensionStart = firstDimensionStart;
        FirstDimensionEnd = firstDimensionEnd;
        SecondDimensionStart = secondDimensionStart;
        SecondDimensionEnd = secondDimensionEnd;
    }

    public Bounds2D(int firstDimensionEnd, int secondDimensionEnd) : this(0, firstDimensionEnd, 0, secondDimensionEnd) {
    }

    public bool ValueInBounds(int x, int y) {
        return x >= FirstDimensionStart & x < FirstDimensionEnd &&
               y >= SecondDimensionStart & y < SecondDimensionEnd;
    }
    
    public bool ValueInBounds((int x, int y) item) {
        return ValueInBounds(item.x, item.y);
    }
}