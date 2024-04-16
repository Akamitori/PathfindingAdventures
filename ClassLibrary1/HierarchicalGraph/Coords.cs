namespace ClassLibrary1.HierachicalGraph;

public struct Coords : IEquatable<Coords> {
    public int X;
    public int Y;

    public Coords(int x, int y) {
        X = x;
        Y = y;
    }

    public override bool Equals(object? obj) {
        return obj is Coords other && Equals(other);
    }

    public bool Equals(Coords other) {
        return X == other.X && Y == other.Y;
    }

    public override int GetHashCode() {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Coords left, Coords right) {
        return left.Equals(right);
    }

    public static bool operator !=(Coords left, Coords right) {
        return !left.Equals(right);
    }

    public override string ToString() {
        return $"({X},{Y})";
    }
}