namespace ClassLibrary1;

public class AStar<T> {
    private readonly PriorityQueue<int, float> frontier = new PriorityQueue<int, float>();
    private Dictionary<int, int> cameFrom = new Dictionary<int, int>();
    private Dictionary<int, float> costSoFar = new Dictionary<int, float>();

    private readonly Graph.Graph<T> g;
    private readonly int startId;
    private readonly int endId;

    private bool completed = false;

    private readonly T EndPointData;

    private readonly Func<T, T, int> heuristic;
    private readonly Func<T, bool> filter;

    public AStar(Graph.Graph<T> g, int startId, int endId, Func<T, T, int> heuristic,
        Func<T, bool> filter) {
        this.g = g;
        this.startId = startId;
        this.endId = endId;
        frontier.Enqueue(this.startId, 0);
        completed = false;
        cameFrom[this.startId] = -1;
        costSoFar[startId] = 0;
        EndPointData = g.ConvertFromId(endId);
        this.heuristic = heuristic;
        this.filter = filter;
    }

    public AStar(Graph.Graph<T> g, T startNodeData, T endNodeData, Func<T, T, int> heuristic,
        Func<T, bool> filter) :
        this(g,
            g.ConvertToId(startNodeData),
            g.ConvertToId(endNodeData),
            heuristic, filter) {
    }

    public bool ExecuteToCompletion(out int[] path, out float cost) {
        while (!this.ExecuteStep()) {
            ;
        }


        path = GetPath();
        cost = default;

        if (path == Array.Empty<int>()) {
            return false;
        }

        cost = GetPathCost();
        return true;
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

                var tuples = g.ConvertFromId(next);
                if (!filter(tuples)) {
                    continue;
                }

                var newCost = costSoFar[currentNode] + edgeToNeighbor.cost;

                if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next]) {
                    costSoFar[next] = newCost;
                    var nextCoords = g.ConvertFromId(next);
                    var priority = newCost + heuristic(EndPointData, nextCoords);
                    frontier.Enqueue(next, priority);
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

    public float GetPathCost() {
        if (!completed || !costSoFar.TryGetValue(endId, out var cost)) {
            return -1;
        }

        return cost;
    }
}