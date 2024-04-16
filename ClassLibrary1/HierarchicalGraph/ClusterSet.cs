using ClassLibrary1.GraphBuilder;
using ClassLibrary1.HierachicalGraph;

namespace ClassLibrary1.HierarchicalGraph;

public class ClusterSet {
    public int[,] ClusterAdjacency;

    private Cluster[,] clusters;

    private int emptyNeighborValue = -1;

    private readonly List<EntranceSet> entranceSet;
    private readonly int[,] map;
    public readonly int clusterSize;
    private readonly Bounds2D mapBounds2D;

    public IEnumerable<EntranceSet> ClusterEntranceSets => entranceSet;

    public ClusterSet(int[,] map, int clusterSize) {
        this.map = map;
        this.clusterSize = clusterSize;
        mapBounds2D = new Bounds2D(map.GetLength(0), map.GetLength(1));
        BuildClusters();
        entranceSet = CalculateEntrances();
    }

    private void BuildClusters() {
        var mapRows = mapBounds2D.FirstDimensionEnd;
        var mapColumns = mapBounds2D.SecondDimensionEnd;
        var clusterRows = mapRows / clusterSize;
        var clusterColumns = mapColumns / clusterSize;

        var clusterTotal = clusterRows * clusterColumns;
        clusters = new Cluster[clusterRows, clusterColumns];

        var clusterCounter = 0;

        for (var startRow = 0; startRow < mapRows; startRow += clusterSize) {
            for (var startColumn = 0; startColumn < mapColumns; startColumn += clusterSize) {
                var sectorRows = Math.Min(clusterSize, mapRows - startRow);
                var sectorColumns = Math.Min(clusterSize, mapColumns - startColumn);


                var endRow = startRow + sectorRows - 1;
                var endColumn = startColumn + sectorColumns - 1;

                var cluster = new Cluster(clusterCounter, sectorRows, sectorColumns, startColumn,
                    endColumn, startRow, endRow);

                var clusterItemCounter = 0;
                for (var localY = 0; localY < sectorRows; localY++) {
                    for (var localX = 0; localX < sectorColumns; localX++) {
                        var y = startRow + localY;
                        var x = startColumn + localX;

                        cluster.AddItem(x, y, clusterItemCounter++);
                    }
                }

                var clusterCoords = ConvertFromId(clusterCounter, clusters.GetLength(1));

                clusters[clusterCoords.Y, clusterCoords.X] = cluster;
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

    private Coords ConvertFromId(int id, int secondDimension) {
        var row = id / secondDimension;
        var column = id % secondDimension;
        return new Coords(column, row);
    }

    public Cluster GetCluster(int clusterId) {
        var clusterCoords = ConvertFromId(clusterId, clusters.GetLength(1));
        return clusters[clusterCoords.Y, clusterCoords.X];
    }

    public Coords GetClusterCoords(int clusterId) {
        return ConvertFromId(clusterId, clusters.GetLength(1));
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
                var clusterCoords = ConvertFromId(item, clusters.GetLength(1));
                yield return clusters[clusterCoords.Y, clusterCoords.X];
            }
        }
    }

    private List<EntranceSet> CalculateEntrances() {
        var hashSet = new HashSet<(int, int)>();
        var entrances = new List<EntranceSet>();


        foreach (var cluster in ClusterItems()) {
            foreach (var neighbor in ClusterNeighbor(cluster.id)) {
                var neighborId = neighbor.id;

                if (hashSet.Contains((neighborId, cluster.id)) ||
                    hashSet.Contains((cluster.id, neighborId))) {
                    continue;
                }

                GetCluster(neighborId);
                var neighborCluster = GetCluster(neighborId);

                var set = BuildEntrances(cluster, neighborCluster);
                entrances.AddRange(set);

                hashSet.Add((neighborCluster.id, cluster.id));
            }
        }

        return entrances;
    }

    private List<EntranceSet> BuildEntrances(
        Cluster cluster, Cluster neighBorCluster) {
        var clusterCoords = GetClusterCoords(cluster.id);
        var neighborClusterCoords = GetClusterCoords(neighBorCluster.id);


        var relativePosition = DetermineAdjacency(clusterCoords, neighborClusterCoords);

        var entrances = new List<EntranceSet>();
        var verticalAdjacency = relativePosition is RelativePosition.Up or RelativePosition.Down;
        var tiles = new List<Coords>();
        var symmetricalTiles = new List<Coords>();
        if (verticalAdjacency) {
            Cluster upperCluster;
            Cluster lowerCluster;
            RelativePosition neighborRelativePosition;
            if (relativePosition == RelativePosition.Up) {
                upperCluster = neighBorCluster;
                lowerCluster = cluster;
                neighborRelativePosition = RelativePosition.Down;
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

                if (tile1 != emptyNeighborValue && tile2 != emptyNeighborValue) {
                    tiles.Add(coords1);
                    symmetricalTiles.Add(coords2);
                }
                else if (symmetricalTiles.Count > 0) {
                    entrances.Add(new EntranceSet(tiles.ToArray(),
                        lowerCluster.id,
                        neighborRelativePosition,
                        symmetricalTiles.ToArray(),
                        upperCluster.id,
                        relativePosition)
                    );
                    tiles.Clear();
                    symmetricalTiles.Clear();
                }
            }

            if (tiles.Count > 0) {
                entrances.Add(new EntranceSet(tiles.ToArray(),
                        lowerCluster.id,
                        neighborRelativePosition, // if our neighbor is down so is our entrance and vice verca
                        symmetricalTiles.ToArray(),
                        upperCluster.id,
                        relativePosition
                    )
                );
            }

            return entrances;
        }
        else {
            Cluster rightCluster;
            Cluster leftCluster;
            var neighborRelativePosition = RelativePosition.Left;
            if (relativePosition == RelativePosition.Right) {
                rightCluster = neighBorCluster;
                leftCluster = cluster;
            }
            else {
                rightCluster = cluster;
                leftCluster = neighBorCluster;
                neighborRelativePosition = RelativePosition.Right;
            }

            for (var x = 0; x < cluster.subMap.GetLength(1); x++) {
                var coords1 = leftCluster.subMap[x, leftCluster.subMap.GetLength(1) - 1];
                var coords2 = rightCluster.subMap[x, 0];

                var tile1 = map[coords1.Y, coords1.X];
                var tile2 = map[coords2.Y, coords2.X];

                if (tile1 != emptyNeighborValue && tile2 != emptyNeighborValue) {
                    tiles.Add(coords1);
                    symmetricalTiles.Add(coords2);
                }
                else {
                    entrances.Add(new EntranceSet(tiles.ToArray(),
                        leftCluster.id,
                        neighborRelativePosition,
                        symmetricalTiles.ToArray(),
                        rightCluster.id,
                        relativePosition)
                    );
                    tiles.Clear();
                    symmetricalTiles.Clear();
                }
            }

            if (tiles.Count > 0) {
                entrances.Add(new EntranceSet(tiles.ToArray(),
                    leftCluster.id,
                    neighborRelativePosition,
                    symmetricalTiles.ToArray(),
                    rightCluster.id,
                    relativePosition)
                );
            }

            return entrances;
        }
    }


    /// <summary>
    /// Determines the type of adjacency border 2 has to border 1
    /// </summary>
    /// <param name="border1"></param>
    /// <param name="border2"></param>
    /// <returns></returns>
    private RelativePosition DetermineAdjacency(Coords border1, Coords border2) {
        if (border1.Y == border2.Y) {
            return border1.X < border2.X ? RelativePosition.Right : RelativePosition.Left;
        }

        if (border1.X == border2.X) {
            return border1.Y < border2.Y ? RelativePosition.Down : RelativePosition.Up;
        }

        throw new InvalidDataException();
    }
}