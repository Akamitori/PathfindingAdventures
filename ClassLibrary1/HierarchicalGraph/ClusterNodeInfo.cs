using ClassLibrary1.HierachicalGraph;

namespace ClassLibrary1.HierarchicalGraph;

public struct ClusterNodeInfo {
    private int ClusterId;
    public Coords NodeCoords;

    public ClusterNodeInfo(Coords nodeCoords, int clusterId) {
        NodeCoords = nodeCoords;
        ClusterId = clusterId;
    }
    
}