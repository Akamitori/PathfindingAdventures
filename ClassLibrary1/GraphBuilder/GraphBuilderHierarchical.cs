using System.Diagnostics;
using ClassLibrary1.Graph;
using ClassLibrary1.HierachicalGraph;
using ClassLibrary1.HierarchicalGraph;

namespace ClassLibrary1.GraphBuilder;

public class GraphBuilderFromEntrancesAndClusters : IGraphBuilder<ClusterNodeInfo> {
    private readonly ClusterSet clusterSet;
    private readonly Dictionary<Cluster, Dictionary<RelativePosition, List<EntranceSet>>> entranceSets;
    private readonly int clusterSize;

    private readonly Graph<Coords> lowLevelGraph;


    public GraphBuilderFromEntrancesAndClusters(ClusterSet clusterSet,
        Dictionary<Cluster, Dictionary<RelativePosition, List<EntranceSet>>> entranceSets
        , int clusterSize
        , Graph<Coords> lowLevelGraph
    ) {
        this.clusterSet = clusterSet;
        this.entranceSets = entranceSets;
        this.clusterSize = clusterSize;
        this.lowLevelGraph = lowLevelGraph;
    }

    private const int MultipleEntranceThreshold = 6;

    public Graph.Graph<ClusterNodeInfo> BuildGraph() {
        // 
        // we need to create actual nodes

        // what nodes are those
        // all the entrance points are nodes
        // so we have

        // for each cluster make the intra nodes


        // how many nodes will we create?

        // we 2 info crucial for the nodes.
        // the cluster they belong to so we can create intra edges
        // and ofc all the neighboring clusters so we can actually work on those.


        //foreach ()


        // the big question is how do we id those nodes?
        // one idea would be cluster ID +
        // in low level graph we just use a coordinate calculation
        // in this we are gonna need a different way to do things.
        // foreach (var entranceSets in entranceSet.Values.SelectMany(c => c.Values)) {
        //     foreach (var es in entranceSets) {
        //         totalNodes += es.Cluster1Entrance.length;
        //     }
        // }

        var maxNodesPerCluster = clusterSize * 4 - 4;
        var maxNodes = maxNodesPerCluster * clusterSet.ClusterItems().Count();

        var maxInternalNeighbours = maxNodesPerCluster - 1;
        var maxExternalNeighbors = 2;
        var totalNeighbors = maxInternalNeighbours + maxExternalNeighbors;
        var adjacencyGraph = new EdgeInfo[maxNodes, totalNeighbors];

        var nodes = new Node<ClusterNodeInfo>[maxNodes];

        var nodeId = 0;
        //those 2 thins are needed for proper tracking of the internal state of the node;
        var neighborCount = new Dictionary<int, int>();
        var nodeIds = new Dictionary<Coords, int>();
        //those 2 thins are needed for proper tracking of the internal state of the node;
        var clusterEntrances = new Dictionary<int, HashSet<Coords>>();


        foreach (var entrances in entranceSets.Values.SelectMany(c => c.Values)) {
            foreach (var entranceSet in entrances) {
                var entranceLength = entranceSet.length;
                var maxEntranceCount = entranceSet.length < MultipleEntranceThreshold ? 1 : 2;
                var totalEntrances = 0;


                // inter edges
                for (var i = 0; i < entranceLength; i++) {
                    var tile = entranceSet.Cluster1Entrance.Tiles[i];
                    var tilesOwnerId = entranceSet.Cluster1Entrance.TileOwner;
                    var tileId = AddTileToNodeSet(nodeIds, tile, tilesOwnerId, nodes, ref nodeId);

                    if (!clusterEntrances.TryGetValue(tilesOwnerId, out var coordsSet)) {
                        coordsSet = new HashSet<Coords>();
                        clusterEntrances[tilesOwnerId] = coordsSet;
                    }

                    coordsSet.Add(tile);

                    var symmetricalTile = entranceSet.Cluster2Entrance.Tiles[i];
                    var symTilesOwnerId = entranceSet.Cluster2Entrance.TileOwner;
                    var symmetricalTileId =
                        AddTileToNodeSet(nodeIds, symmetricalTile, symTilesOwnerId, nodes, ref nodeId);

                    if (!clusterEntrances.TryGetValue(symTilesOwnerId, out var symCoordsSet)) {
                        symCoordsSet = new HashSet<Coords>();
                        clusterEntrances[symTilesOwnerId] = symCoordsSet;
                    }

                    symCoordsSet.Add(symmetricalTile);


                    var placedAllTheEntrances = totalEntrances >= maxEntranceCount;

                    if (placedAllTheEntrances) {
                        continue;
                    }

                    if (totalEntrances > 0 && i != entranceLength - 1) {
                        continue;
                    }

                    var lowLevelTileId = lowLevelGraph.ConvertToId(tile);
                    var lowLevelSymTileId = lowLevelGraph.ConvertToId(symmetricalTile);

                    var cost1 = 1f;
                    var cost2 = 1f;
                    for (var k = 0; k < lowLevelGraph.Adjacency.GetLength(1); k++) {
                        var tile1Cost = lowLevelGraph.Adjacency[lowLevelTileId, k];
                        var tile2Cost = lowLevelGraph.Adjacency[lowLevelSymTileId, k];

                        if (tile1Cost.neighbor == lowLevelSymTileId) {
                            cost1 += tile1Cost.cost;
                        }

                        if (tile2Cost.neighbor == lowLevelTileId) {
                            cost1 += tile2Cost.cost;
                        }
                    }


                    AddNeighbor(neighborCount, tileId, adjacencyGraph, symmetricalTileId, cost1);
                    AddNeighbor(neighborCount, symmetricalTileId, adjacencyGraph, tileId, cost2);

                    totalEntrances++;
                }

                // time to create the legendary intra edges
                foreach (var (clusterId, entranceCoords) in clusterEntrances) {
                    var cluster = clusterSet.GetCluster(clusterId);
                    var pairs = GetAllUniquePairs(entranceCoords);


                    foreach (var (start, end) in pairs) {
                        TryAddEdge(start, end, cluster, nodeIds, neighborCount, adjacencyGraph);
                        TryAddEdge(end, start, cluster, nodeIds, neighborCount, adjacencyGraph);
                        //do an A*, save the costs!
                    }
                }
                
                clusterEntrances.Clear();
                // foreach (var coords in entranceSet.Cluster1Entrance.Tiles) {
                //     nodes[id] = new Node<Coords>(coords, id);
                //     nodeIds[coords] = id;
                // }
            }
        }

        return new ClusterGraph(nodes, adjacencyGraph, nodeIds, neighborCount);


        // we need to iterate through each entrance set.
        // for symmetrical e


        //how many edges will this have max

        // for intra it will be cluster size*cluster size -2 + 1 if we have outer edges.


        //we get a set of coords that are our entrances
        // those set of coords are our nodes

        //we can use those set of coords to then perform an A* on the original graph in order to just properly cache the distance so we can have costs for the actual nodes

        throw new NotImplementedException();
    }

    private void TryAddEdge(Coords A, Coords B, Cluster cluster, Dictionary<Coords, int> nodeIds,
        Dictionary<int, int> neighborCount,
        EdgeInfo[,] adjacencyGraph) {
        var aStarAToB = new AStar<Coords>(lowLevelGraph, A, B,
            ManhattanDistanceHeuristic,
            cluster.Contains);


        if (!aStarAToB.ExecuteToCompletion(out var path, out var cost)) {
            return;
        }

        var startId = nodeIds[A];
        var endId = nodeIds[B];

        AddNeighbor(neighborCount, startId, adjacencyGraph, endId, cost);
        int ManhattanDistanceHeuristic(Coords a, Coords b) => Math.Abs(a.X - b.X) + Math.Abs(a.Y - b.Y);
    }

    private static int AddTileToNodeSet(IDictionary<Coords, int> nodeIds, Coords tile, int tilesOwnerId,
        Node<ClusterNodeInfo>[] nodes, ref int nodeId) {
        if (nodeIds.TryGetValue(tile, out var tileId)) {
            return tileId;
        }

        tileId = nodeId++;
        nodeIds[tile] = tileId;
        var clusterNodeInfo = new ClusterNodeInfo(tile, tilesOwnerId);
        nodes[tileId] = new Node<ClusterNodeInfo>(clusterNodeInfo, tilesOwnerId);

        return tileId;
    }

    private static List<(Coords, Coords)> GetAllUniquePairs(HashSet<Coords> coordsSet) {
        var set1 = coordsSet.ToArray();

        var result = new List<(Coords, Coords)>();

        for (var i = 0; i < set1.Length; i++) {
            var item1 = set1[i];
            for (var j = i + 1; j < set1.Length; j++) {
                var item2 = set1[j];
                result.Add((item1, item2));
            }
        }

        return result;
    }

    private static void AddNeighbor(
        Dictionary<int, int> neighborCount,
        int tileId, EdgeInfo[,] adjacencyGraph,
        int neighborId, float cost) {
        var neighbor1Index = neighborCount.GetValueOrDefault(tileId, 0);
        neighborCount[tileId] = neighbor1Index + 1;

        if (tileId == 1 && neighborId == 3) {
            ;
        }
        
        adjacencyGraph[tileId, neighbor1Index] = new EdgeInfo() {
            cost = cost,
            enabled = true,
            neighbor = neighborId
        };
    }
}