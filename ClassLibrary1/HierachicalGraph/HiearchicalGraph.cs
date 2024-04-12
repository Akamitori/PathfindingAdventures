namespace ClassLibrary1;

public class HiearchicalGraph {
    // assume a square map for the sake of it

    public int[,] ClusterAdjacency;

    public Cluster[,] Clusters;

    public int[,] map;
    private int emptyNeighborValue = -1;

    private const int oneEdgeLimit = 6;

    public HiearchicalGraph(int[,] map, int clusterSize) {
        this.map = map;
        BuildClusters(map, clusterSize);
        var entranceSet = CalculateEntrances();
        // create a graph that has the following connections
        
        // intra edges and connections between clusters
        
        
        // we need to make a graph
        // each node will have [cluster size * cluster size -1] internal edges
        // and max 2 inter edges
        // each cluster can have maxiumum [cluster size * 4] internal nodes
        // so in total the max number of nodes is  [cluster size * 4 * number of clusters]
        
        //what this means for our representation
        //

        foreach (var cluster in Clusters) {
            var entrances = entranceSet[cluster.id];

            foreach (var entrance in entrances) {
                //var entrancesToUse=cluster
            }
        }
    }

    private Dictionary<int, Dictionary<Adjacency, List<Entrance>>> CalculateEntrances() {
        var hashSet = new HashSet<(int, int)>();
        var entrances = new Dictionary<int, Dictionary<Adjacency, List<Entrance>>>();

        for (var i = 0; i < Clusters.GetLength(0); i++) {
            for (var j = 0; j < Clusters.GetLength(1); j++) {
                var cluster = Clusters[i, j];


                for (var k = 0; k < ClusterAdjacency.GetLength(1); k++) {
                    var adjacentClusterId = ClusterAdjacency[cluster.id, k];

                    if (adjacentClusterId == emptyNeighborValue) {
                        continue;
                    }

                    if (hashSet.Contains((adjacentClusterId, cluster.id)) ||
                        hashSet.Contains((cluster.id, adjacentClusterId))) {
                        continue;
                    }

                    var result = BuildEntrances(cluster, adjacentClusterId);


                    var clusterEntrances =
                        entrances.GetValueOrDefault(cluster.id, new Dictionary<Adjacency, List<Entrance>>());
                    var neighborEntrances = entrances.GetValueOrDefault(adjacentClusterId,
                        new Dictionary<Adjacency, List<Entrance>>());

                    clusterEntrances.Add(result.positionInCluster1, result.entrances);
                    neighborEntrances.Add(result.positionInCluster2, result.entrances);


                    hashSet.Add((adjacentClusterId, cluster.id));
                }
            }
        }

        return entrances;
    }

    private (List<Entrance> entrances, Adjacency positionInCluster1, Adjacency positionInCluster2) BuildEntrances(
        Cluster cluster, int neighborClusterId) {
        var clustersColumns = Clusters.GetLength(1);
        var border1 = ConvertFromId(cluster.id, clustersColumns);
        var border2 = ConvertFromId(neighborClusterId, clustersColumns);
        var neighBorCluster = Clusters[border2.row, border2.column];

        var adjacency = DetermineAdjacency(border1, border2);

        var entrances = new List<Entrance>();
        var verticalAdjacency = adjacency is Adjacency.Up or Adjacency.Down;
        var entranceLength = 0;
        var tiles = new List<Coords>();
        var symmetricalTiles = new List<Coords>();
        if (verticalAdjacency) {
            Cluster upperCluster;
            Cluster lowerCluster;
            var neighborRelativePosition = Adjacency.Down;
            if (adjacency == Adjacency.Up) {
                upperCluster = neighBorCluster;
                lowerCluster = cluster;
            }
            else {
                upperCluster = cluster;
                lowerCluster = neighBorCluster;
                neighborRelativePosition = Adjacency.Up;
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
            var neighborRelativePosition = Adjacency.Left;
            if (adjacency == Adjacency.Right) {
                rightCluster = neighBorCluster;
                leftCluster = cluster;
            }
            else {
                rightCluster = cluster;
                leftCluster = neighBorCluster;
                neighborRelativePosition = Adjacency.Right;
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

    public enum Adjacency {
        Up,
        Down,
        Left,
        Right
    }


    /// <summary>
    /// Determines the type of adjacency border 2 has to border 1
    /// </summary>
    /// <param name="border1"></param>
    /// <param name="border2"></param>
    /// <returns></returns>
    private Adjacency DetermineAdjacency((int row, int column) border1, (int row, int column) border2) {
        if (border1.row == border2.row) {
            return border1.column < border2.column ? Adjacency.Right : Adjacency.Left;
        }

        if (border1.column == border2.column) {
            return border1.row < border2.row ? Adjacency.Down : Adjacency.Up;
        }

        throw new InvalidDataException();
    }


    public void BuildClusters(int[,] map, int clusterSize) {
        var mapRows = map.GetLength(0);
        var mapColumns = map.GetLength(1);

        var clusterRows = mapRows / clusterSize;
        var clusterColumns = mapColumns / clusterSize;

        var clusterTotal = clusterRows * clusterColumns;
        Clusters = new Cluster[clusterRows, clusterColumns];

        var clusterCounter = 0;
        var yLength = map.GetLength(0);
        var xLength = map.GetLength(1);

        var sectorRows = clusterSize;
        var sectorColumns = clusterSize;

        for (var startY = 0; startY < yLength; startY += clusterSize) {
            for (var startX = 0; startX < xLength; startX += clusterSize) {
                var totalRows = Math.Min(sectorRows, yLength - startY);
                var totalColumns = Math.Min(sectorColumns, xLength - startX);


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

                var columns = Clusters.GetLength(1);
                var row = clusterCounter / columns;
                var column = clusterCounter % columns;

                Clusters[row, column] = cluster;
                clusterCounter++;
            }
        }

        ClusterAdjacency = new int[clusterTotal, 4];

        for (var i = 0; i < ClusterAdjacency.GetLength(0); i++) {
            for (var j = 0; j < ClusterAdjacency.GetLength(1); j++) {
                ClusterAdjacency[i, j] = emptyNeighborValue;
            }
        }

        for (var i = 0; i < Clusters.GetLength(0); i++) {
            for (var j = 0; j < Clusters.GetLength(1); j++) {
                var neighborIndexes = GetNeighborIndexes(i, j, (clusterRows, clusterColumns));
                var id = ConvertToId(i, j, clusterColumns);
                for (var k = 0; k < neighborIndexes.Count; k++) {
                    var (x, y) = neighborIndexes[k];
                    ClusterAdjacency[id, k] = ConvertToId(x, y, clusterColumns);
                }
            }
        }
    }

    public int ConvertToId(int x, int y, int secondDimension) {
        return x * secondDimension + y;
    }

    public (int row, int column) ConvertFromId(int id, int secondDimension) {
        var columns = secondDimension;
        var row = id / columns;
        var column = id % columns;
        return (row, column);
    }

    private List<(int, int)> GetNeighborIndexes(int i, int j, (int, int) array) {
        var up = (-1 + i, j);
        var down = (1 + i, j);
        var left = (i, -1 + j);
        var right = (i, 1 + j);


        var result = new List<(int, int)>();
        AddIfInBounds(left, result, array);
        AddIfInBounds(right, result, array);
        AddIfInBounds(up, result, array);
        AddIfInBounds(down, result, array);

        return result;
    }

    private void AddIfInBounds((int, int) itemToAdd, ICollection<(int, int)> result, (int, int) array) {
        if (InBounds(itemToAdd, array)) {
            result.Add(itemToAdd);
        }
    }

    private bool InBounds(int x, int y, (int x, int y) array) {
        return x >= 0 && x < array.x
                      && y >= 0 && y < array.y;
    }

    private bool InBounds((int x, int y) item, (int, int) array) {
        return InBounds(item.x, item.y, array);
    }
}