namespace ClassLibrary1;

public static class GraphHelperMethods {
    public static int ConvertToId(int x, int y, int gridColumns) {
        return x * gridColumns + y;
    }

    public static (int, int) ConvertFromId(int id, int gridColumns) {
        var row = id / gridColumns;
        var column = id % gridColumns;
        return (row, column);
    }

    public static bool InBounds(int x, int y, int rowCount, int columnCount) {
        return x >= 0 && x < rowCount
                      && y >= 0 && y <columnCount;
    }

    public static bool InBounds((int x, int y) item, int rowCount, int columnCount) {
        return InBounds(item.x, item.y, rowCount, columnCount);
    }
}