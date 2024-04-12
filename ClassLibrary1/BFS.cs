namespace ClassLibrary1;

public class BFS {
    private readonly Queue<int> nodesToProcess = new Queue<int>();
    private readonly HashSet<int> nodesAlreadyVisited = new HashSet<int>();
    private Dictionary<int, int> cameFrom = new Dictionary<int, int>();

    private readonly Graph g;
    private readonly int startId;
    private readonly int endId;

    private bool completed = false;

    public BFS(Graph g, int start, int end) {
        this.g = g;
        startId = g.Nodes[start].id;
        endId = g.Nodes[end].id;
        nodesToProcess.Enqueue(startId);
        completed = false;
        cameFrom[startId] = -1;
    }

    public BFS(Graph g, int startX, int startY, int endX, int endY) :
        this(g,
            GraphHelperMethods.ConvertToId(startX, startY, g.Map.GetLength(1)),
            GraphHelperMethods.ConvertToId(endX, endY, g.Map.GetLength(1))
        ) {
    }

    public bool ExecuteStep() {
        if (!completed && nodesToProcess.TryDequeue(out var index)) {
            if (index == endId) {
                completed = true;
                return true;
            }

            var neighbors = g.GetNeighborsOfNode(index);

            for (var i = 0; i < neighbors.Count; i++) {
                var edgeToNeighbor = neighbors[i];
                if (nodesAlreadyVisited.Contains(edgeToNeighbor.neighbor)) {
                    continue;
                }

                cameFrom[edgeToNeighbor.neighbor] = index;

                nodesToProcess.Enqueue(edgeToNeighbor.neighbor);
            }


            nodesAlreadyVisited.Add(index);
            return false;
        }

        completed = true;
        return true;
    }

    public int[] GetPath() {
        if (!completed || !cameFrom.ContainsKey(endId)) {
            return Array.Empty<int>();
        }

        var path = new List<int>();
        var current = endId;
        while (current != startId) {
            path.Add(current);
            current = cameFrom[current];
        }

        path.Add(startId);
        path.Reverse();
        return path.ToArray();
    }
}