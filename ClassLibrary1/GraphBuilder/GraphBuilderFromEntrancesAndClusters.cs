// using ClassLibrary1.HierachicalGraph;
// using ClassLibrary1.HierarchicalGraph;
//
// namespace ClassLibrary1.GraphBuilder;
//
// public class GraphBuilderFromEntrancesAndClusters : IGraphBuilder {
//     private readonly ClusterSet clusterSet;
//     private readonly Dictionary<Cluster, Dictionary<RelativePosition, List<Entrance>>> entranceSet;
//
//
//     public GraphBuilderFromEntrancesAndClusters(ClusterSet clusterSet, Dictionary<Cluster, Dictionary<RelativePosition, List<Entrance>>> entranceSet) {
//         this.clusterSet = clusterSet;
//         this.entranceSet = entranceSet;
//     }
//     
//     public Graph.Graph BuildGraph() {
//         foreach (var cluster in clusterSet.ClusterItems()) {
//             // build the intra edges
//             
//         }
//     }
// }