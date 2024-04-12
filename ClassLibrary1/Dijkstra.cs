namespace ClassLibrary1;

public class Dijkstra {
    private readonly PriorityQueue<int, int> frontier = new PriorityQueue<int, int>();
    private Dictionary<int, int> cameFrom = new Dictionary<int, int>();
    private Dictionary<int, int> costSoFar = new Dictionary<int, int>();

    private readonly Graph g;
    private readonly int startId;
    private readonly int endId;

    private bool completed = false;

    public Dijkstra(Graph g, int start, int end) {
        this.g = g;
        startId = g.Nodes[start].id;
        endId = g.Nodes[end].id;
        frontier.Enqueue(startId, 0);
        completed = false;
        cameFrom[startId] = -1;
        costSoFar[start] = 0;
    }

    public Dijkstra(Graph g, int startX, int startY, int endX, int endY) :
        this(g,
            GraphHelperMethods.ConvertToId(startX, startY, g.Map.GetLength(1)),
            GraphHelperMethods.ConvertToId(endX, endY, g.Map.GetLength(1))
        ) {
    }

    public bool ExecuteStep() {
        if (!completed && frontier.TryDequeue(out var currentNode, out _)) {
            if (currentNode == endId) {
                completed = true;
                return true;
            }

            var edgeToNeighbors = g.GetNeighborsOfNode(currentNode);

            for (var i = 0; i < edgeToNeighbors.Count; i++) {
                var edgeToNeighbor = edgeToNeighbors[i];

                var next = edgeToNeighbor.neighbor;
                var newCost = costSoFar[currentNode] + edgeToNeighbor.cost;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    frontier.Enqueue(next, newCost);
                    cameFrom[next] = currentNode;
                }
            }

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