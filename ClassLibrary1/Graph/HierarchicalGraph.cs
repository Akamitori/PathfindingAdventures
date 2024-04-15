using ClassLibrary1.HierachicalGraph;
using ClassLibrary1.HierarchicalGraph;

namespace ClassLibrary1.Graph;

public class HierarchicalGraph : Graph<ClusterNodeInfo> {
    private readonly Dictionary<Coords, int> nodeIdsFromCoords;
    private readonly Dictionary<int, int> neighborCount;
    
    private Graph.Graph<Coords> LowLevelGraph;


    public HierarchicalGraph(Node<ClusterNodeInfo>[] nodes, EdgeInfo[,] adjacencyGraph, Dictionary<Coords, int> nodeIds, Dictionary<int, int> neighborCount, Graph<Coords> lowLevelGraph)  : base(nodes, adjacencyGraph) {
        this.nodeIdsFromCoords = nodeIds;
        this.neighborCount = neighborCount;
        LowLevelGraph = lowLevelGraph;
    }


    public override int ConvertToId(ClusterNodeInfo nodeInfo) {
        return nodeIdsFromCoords[nodeInfo.NodeCoords];
    }

    public override ClusterNodeInfo ConvertFromId(int id) {
        return Nodes[id].Data;
    }
}