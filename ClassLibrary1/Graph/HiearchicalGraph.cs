using ClassLibrary1.GraphBuilder;
using ClassLibrary1.HierachicalGraph;

namespace ClassLibrary1.HierarchicalGraph;

public class HiearchicalGraph {
    // assume a square map for the sake of it

    public ClusterSet ClusterSet;

    public int[,] map;
    private int emptyNeighborValue = -1;

    private const int oneEdgeLimit = 6;

    private Graph.Graph<Coords> LowLevelGraph;

    //private Graph.Graph AbstractGraph;

    public HiearchicalGraph(int[,] map, int clusterSize) {
        this.map = map;
        
        var graphBuilder = new GraphBuilderFromMapWithDiagonals(map);
        LowLevelGraph = graphBuilder.BuildGraph();

        ClusterSet = new ClusterSet(map, clusterSize, LowLevelGraph);
        
        //create all the freaking nodes
        // and their intra connections
        
        
        
        // create a graph that has the following connections

        // intra edges and connections between clusters


        // we need to make a graph
        // each node will have [cluster size * cluster size -1] internal edges
        // and max 2 inter edges
        // each cluster can have maxiumum [cluster size * 4] internal nodes
        // so in total the max number of nodes is  [cluster size * 4 * number of clusters]

        //what this means for our representation
        //

        //var graphBuilderEntrances = new GraphBuilderFromEntrances(entranceSet, clusterSize, Clusters);
    }

   
}