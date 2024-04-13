using ClassLibrary1.GraphBuilder;
using ClassLibrary1.HierachicalGraph;

namespace ClassLibrary1.HierarchicalGraph;

public class ClusterSet {
    public int[,] ClusterAdjacency;

    private Cluster[,] clusters;

    private int emptyNeighborValue = -1;

    private readonly Dictionary<Cluster, Dictionary<RelativePosition, List<Entrance>>> entranceSet;
    private readonly int[,] map;
    private readonly int clusterSize;
    private readonly Bounds2D mapBounds2D;

    public ClusterSet(int[,] map, int clusterSize) {
        this.map = map;
        this.clusterSize = clusterSize;
        mapBounds2D = new Bounds2D(map.GetLength(0), map.GetLength(1));
        BuildClusters();
        entranceSet = CalculateEntrances();
        //var builder = new GraphBuilderFromEntrancesAndClusters(this, entranceSet, clusterSize);
        //var abstractGraph = builder.BuildGraph();
    }


    private void BuildClusters() {
        var mapRows = mapBounds2D.FirstDimensionEnd;
        var mapColumns = mapBounds2D.SecondDimensionEnd;
        var clusterRows = mapRows / clusterSize;
        var clusterColumns = mapColumns / clusterSize;

        var clusterTotal = clusterRows * clusterColumns;
        clusters = new Cluster[clusterRows, clusterColumns];

        var clusterCounter = 0;
        var sectorRows = clusterSize;
        var sectorColumns = clusterSize;

        for (var startY = 0; startY < mapRows; startY += clusterSize) {
            for (var startX = 0; startX < mapColumns; startX += clusterSize) {
                var totalRows = Math.Min(sectorRows, mapRows - startY);
                var totalColumns = Math.Min(sectorColumns, mapColumns - startX);


                var endY = startY + totalRows - 1;
                var endX = startX + totalColumns - 1;

                var cluster = new Cluster(clusterCounter, totalRows, totalColumns, startX, endX, startY, endY);

                var clusterItemCounter = 0;
                for (var localY = 0; localY < totalRows; localY++) {
                    for (var localX = 0; localX < totalColumns; localX++) {
                        var y = startY + localY;
                        var x = startX + localX;

                        cluster.AddItem(y, x, clusterItemCounter++);
                    }
                }

                var clusterCoords = ConvertFromId(clusterCounter, clusters.GetLength(1));

                clusters[clusterCoords.row, clusterCoords.column] = cluster;
                clusterCounter++;
            }
        }

        ClusterAdjacency = new int[clusterTotal, 4];

        for (var i = 0; i < ClusterAdjacency.GetLength(0); i++) {
            for (var j = 0; j < ClusterAdjacency.GetLength(1); j++) {
                ClusterAdjacency[i, j] = emptyNeighborValue;
            }
        }

        var clusterBounds = new Bounds2D(clusterRows, clusterColumns);
        for (var i = 0; i < clusters.GetLength(0); i++) {
            for (var j = 0; j < clusters.GetLength(1); j++) {
                var neighborIndexes = GetNeighborIndexes(i, j, clusterBounds);
                var id = ConvertToId(i, j, clusterColumns);
                for (var k = 0; k < neighborIndexes.Count; k++) {
                    var (x, y) = neighborIndexes[k];
                    ClusterAdjacency[id, k] = ConvertToId(x, y, clusterColumns);
                }
            }
        }
    }

    private List<(int, int)> GetNeighborIndexes(int i, int j, Bounds2D arrayBounds2D) {
        var itemSet = new (int, int)[] {
            // var up = 
            (-1 + i, j),
            // var down = 
            (1 + i, j),
            // var left = 
            (i, -1 + j),
            // var right  
            (i, 1 + j)
        };
        return itemSet.Where(c => arrayBounds2D.ValueInBounds(c)).ToList();
    }


    private int ConvertToId(int x, int y, int secondDimension) {
        return x * secondDimension + y;
    }

    private (int row, int column) ConvertFromId(int id, int secondDimension) {
        var columns = secondDimension;
        var row = id / columns;
        var column = id % columns;
        return (row, column);
    }

    public Cluster GetCluster(int clusterId) {
        var (row, column) = ConvertFromId(clusterId, mapBounds2D.SecondDimensionEnd);
        return clusters[row, column];
    }

    public (int row, int column) GetClusterCoords(int clusterId) {
        return ConvertFromId(clusterId, mapBounds2D.SecondDimensionEnd);
    }

    public IEnumerable<Cluster> ClusterItems() {
        for (var i = 0; i < clusters.GetLength(0); i++) {
            for (var j = 0; j < clusters.GetLength(1); j++) {
                yield return clusters[i, j];
            }
        }
    }

    public IEnumerable<Cluster> ClusterNeighbor(int clusterId) {
        for (var i = 0; i < ClusterAdjacency.GetLength(1); i++) {
            var item = ClusterAdjacency[clusterId, i];
            var hasNeighbor = item >= 0;
            if (hasNeighbor) {
                var (row, column) = ConvertFromId(item, mapBounds2D.SecondDimensionEnd);
                yield return clusters[row, column];
            }
        }
    }

    private Dictionary<Cluster, Dictionary<RelativePosition, List<Entrance>>> CalculateEntrances() {
        var hashSet = new HashSet<(int, int)>();
        var entrances = new Dictionary<Cluster, Dictionary<RelativePosition, List<Entrance>>>();


        foreach (var cluster in ClusterItems()) {
            foreach (var neighbor in ClusterNeighbor(cluster.id)) {
                var neighborId = neighbor.id;

                if (hashSet.Contains((neighborId, cluster.id)) ||
                    hashSet.Contains((cluster.id, neighborId))) {
                    continue;
                }

                GetCluster(neighborId);
                var neighborCluster = GetCluster(neighborId);


                var result = BuildEntrances(cluster, neighborCluster);

                var clusterEntrances =
                    entrances.GetValueOrDefault(cluster, new Dictionary<RelativePosition, List<Entrance>>());
                var neighborEntrances = entrances.GetValueOrDefault(neighborCluster,
                    new Dictionary<RelativePosition, List<Entrance>>());

                clusterEntrances.Add(result.positionInCluster1, result.entrances);
                neighborEntrances.Add(result.positionInCluster2, result.entrances);


                hashSet.Add((neighborCluster.id, cluster.id));
            }
        }

        return entrances;
    }

    private (List<Entrance> entrances, RelativePosition positionInCluster1, RelativePosition positionInCluster2)
        BuildEntrances(
            Cluster cluster, Cluster neighBorCluster) {
        var clusterCoords = GetClusterCoords(cluster.id);
        var neighborClusterCoords = GetClusterCoords(neighBorCluster.id);


        var adjacency = DetermineAdjacency(clusterCoords, neighborClusterCoords);

        var entrances = new List<Entrance>();
        var verticalAdjacency = adjacency is RelativePosition.Up or RelativePosition.Down;
        var entranceLength = 0;
        var tiles = new List<Coords>();
        var symmetricalTiles = new List<Coords>();
        if (verticalAdjacency) {
            Cluster upperCluster;
            Cluster lowerCluster;
            var neighborRelativePosition = RelativePosition.Down;
            if (adjacency == RelativePosition.Up) {
                upperCluster = neighBorCluster;
                lowerCluster = cluster;
            }
            else {
                upperCluster = cluster;
                lowerCluster = neighBorCluster;
                neighborRelativePosition = RelativePosition.Up;
            }

            for (var x = 0; x < cluster.subMap.GetLength(0); x++) {
                var coords1 = lowerCluster.subMap[0, x];
                var lastIndex = upperCluster.subMap.GetLength(0) - 1;
                var coords2 = upperCluster.subMap[lastIndex, x];

                var tile1 = map[coords1.Y, coords1.X];
                var tile2 = map[coords2.Y, coords2.X];

                if (tile1 > 0 && tile2 > 0) {
                    tiles.Add(coords1);
                    symmetricalTiles.Add(coords2);
                    entranceLength++;
                }
                else if (symmetricalTiles.Count > 0) {
                    entrances.Add(new Entrance(entranceLength, tiles.ToArray(), symmetricalTiles.ToArray()));
                    tiles.Clear();
                    symmetricalTiles.Clear();
                    entranceLength = 0;
                }
            }

            if (entranceLength > 0) {
                entrances.Add(new Entrance(entranceLength, tiles.ToArray(), symmetricalTiles.ToArray()));
            }


            return (entrances, adjacency, neighborRelativePosition);
        }
        else {
            Cluster rightCluster;
            Cluster leftCluster;
            var neighborRelativePosition = RelativePosition.Left;
            if (adjacency == RelativePosition.Right) {
                rightCluster = neighBorCluster;
                leftCluster = cluster;
            }
            else {
                rightCluster = cluster;
                leftCluster = neighBorCluster;
                neighborRelativePosition = RelativePosition.Right;
            }

            for (var x = 0; x < cluster.subMap.GetLength(1); x++) {
                var coords1 = leftCluster.subMap[x, 0];
                var lastIndex = rightCluster.subMap.GetLength(1) - 1;
                var coords2 = rightCluster.subMap[x, lastIndex];

                var tile1 = map[coords1.Y, coords1.X];
                var tile2 = map[coords2.Y, coords2.X];

                if (tile1 > 0 && tile2 > 0) {
                    tiles.Add(coords1);
                    symmetricalTiles.Add(coords2);
                    entranceLength++;
                }
                else {
                    entrances.Add(new Entrance(entranceLength, tiles.ToArray(), symmetricalTiles.ToArray()));
                    tiles.Clear();
                    symmetricalTiles.Clear();
                    entranceLength = 0;
                }
            }

            if (entranceLength > 0) {
                entrances.Add(new Entrance(entranceLength, tiles.ToArray(), symmetricalTiles.ToArray()));
            }


            return (entrances, adjacency, neighborRelativePosition);
        }
    }


    /// <summary>
    /// Determines the type of adjacency border 2 has to border 1
    /// </summary>
    /// <param name="border1"></param>
    /// <param name="border2"></param>
    /// <returns></returns>
    private RelativePosition DetermineAdjacency((int row, int column) border1, (int row, int column) border2) {
        if (border1.row == border2.row) {
            return border1.column < border2.column ? RelativePosition.Right : RelativePosition.Left;
        }

        if (border1.column == border2.column) {
            return border1.row < border2.row ? RelativePosition.Down : RelativePosition.Up;
        }

        throw new InvalidDataException();
    }
}