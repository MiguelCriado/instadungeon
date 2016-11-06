using NUnit.Framework;

public class GraphTest {

	[Test]
    public void Graph_Creation()
    {
        Graph<int> graph = new Graph<int>();
        Assert.True(graph.Count == 0);
    }

    [Test]
    public void Graph_Insert()
    {
        Graph<int> graph = new Graph<int>();
        graph.AddNode(0);
        Assert.True(graph.Contains(0));
    }

    [Test]
    public void Graph_Connect()
    {
        Graph<int> graph = new Graph<int>();
        GraphNode<int> node0 = new GraphNode<int>(0);
        GraphNode<int> node1 = new GraphNode<int>(1);
        graph.AddNode(node0);
        graph.AddNode(node1);
        graph.AddUndirectedEdge(node0, node1, 0);
        Assert.True(node0.Neighbors.Contains(node1) && node1.Neighbors.Contains(node0));
    }
}
