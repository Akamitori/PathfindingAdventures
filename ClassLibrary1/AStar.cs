namespace ClassLibrary1;

public class AStar {
    private readonly PriorityQueue<int, float> frontier = new PriorityQueue<int, float>();
    private Dictionary<int, int> cameFrom = new Dictionary<int, int>();
    private Dictionary<int, float> costSoFar = new Dictionary<int, float>();

    private readonly Graph.Graph g;
    private readonly int startId;
    private readonly int endId;

    private bool completed = false;

    private readonly (int, int) EndPointCoords;

    private readonly Func<(int, int), (int, int), int> heuristic;
    private readonly Func<(int, int), bool> filter;

    public AStar(Graph.Graph g, int start, int end, Func<(int, int), (int, int), int> heuristic,
        Func<(int, int), bool> filter) {
        this.g = g;
        startId = g.Nodes[start].id;
        endId = g.Nodes[end].id;
        frontier.Enqueue(startId, 0);
        completed = false;
        cameFrom[startId] = -1;
        costSoFar[start] = 0;
        EndPointCoords = g.ConvertFromId(end);
        this.heuristic = heuristic;
        this.filter = filter;
    }

    public AStar(Graph.Graph g, int startX, int startY, int endX, int endY, Func<(int, int), (int, int), int> heuristic,
        Func<(int, int), bool> filter) :
        this(g,
            g.ConvertToId(startX, startY),
            g.ConvertToId(endX, endY),
            heuristic, filter) {
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
                    var priority = newCost + heuristic(EndPointCoords, nextCoords);
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

    public float GetPathCost() {
        if (!completed || !costSoFar.TryGetValue(endId, out var cost)) {
            return -1;
        }

        return cost;
    }
}