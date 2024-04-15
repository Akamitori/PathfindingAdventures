namespace ClassLibrary1.GraphBuilder;

public interface IGraphBuilder<T> {
    public Graph.Graph<T> BuildGraph();
}