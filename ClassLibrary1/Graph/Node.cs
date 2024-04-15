namespace ClassLibrary1.Graph;

public struct Node<T> {
    public readonly T Data;
    
    public readonly int Id;

    public Node(T data, int id) {
        this.Data = data;
        this.Id = id;
    }
}