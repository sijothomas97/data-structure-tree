using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace BinaryTree.Api.Tests;

/// <summary>
/// End-to-end tests against the real minimal API pipeline via WebApplicationFactory.
/// Each test builds its own factory (and therefore its own singleton TreeStore),
/// so tests are isolated from each other.
/// </summary>
public sealed class ApiEndpointTests
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private static readonly int[] DemoValues = { 8, 3, 10, 1, 6, 14, 4, 7, 13 };

    private static async Task<HttpClient> CreateClientWithDemoTreeAsync()
    {
        var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        foreach (var value in DemoValues)
        {
            var response = await client.PostAsJsonAsync("/insert", new { value });
            response.EnsureSuccessStatusCode();
        }
        return client;
    }

    private sealed record Traversal(string Order, List<int> Sequence);
    private sealed record Inserted(int Value, bool Added, int Count);
    private sealed record Lca(int First, int Second, int Ancestor);
    private sealed record GraphNode(int Id, int Value, int XIndex, int Depth);
    private sealed record GraphEdge(int From, int To);
    private sealed record Graph(List<GraphNode> Nodes, List<GraphEdge> Edges, int Count, int Height);

    [Fact]
    public async Task Insert_AddsValue_AndRejectsDuplicate()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var first = await client.PostAsJsonAsync("/insert", new { value = 42 });
        var added = await first.Content.ReadFromJsonAsync<Inserted>(Json);
        Assert.NotNull(added);
        Assert.True(added.Added);
        Assert.Equal(1, added.Count);

        var second = await client.PostAsJsonAsync("/insert", new { value = 42 });
        var duplicate = await second.Content.ReadFromJsonAsync<Inserted>(Json);
        Assert.NotNull(duplicate);
        Assert.False(duplicate.Added);
        Assert.Equal(1, duplicate.Count);
    }

    [Theory]
    [InlineData("inorder", new[] { 1, 3, 4, 6, 7, 8, 10, 13, 14 })]
    [InlineData("preorder", new[] { 8, 3, 1, 6, 4, 7, 10, 14, 13 })]
    [InlineData("postorder", new[] { 1, 4, 7, 6, 3, 13, 14, 10, 8 })]
    public async Task Traverse_ReturnsExpectedSequence(string order, int[] expected)
    {
        var client = await CreateClientWithDemoTreeAsync();

        var traversal = await client.GetFromJsonAsync<Traversal>($"/traverse/{order}", Json);

        Assert.NotNull(traversal);
        Assert.Equal(order, traversal.Order);
        Assert.Equal(expected, traversal.Sequence);
    }

    [Fact]
    public async Task Traverse_UnknownOrder_ReturnsBadRequest()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/traverse/sideways");

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Theory]
    [InlineData(1, 6, 3)]
    [InlineData(4, 7, 6)]
    [InlineData(1, 14, 8)]
    [InlineData(3, 4, 3)]
    public async Task Lca_ReturnsLowestCommonAncestor(int first, int second, int expected)
    {
        var client = await CreateClientWithDemoTreeAsync();

        var lca = await client.GetFromJsonAsync<Lca>($"/lca?first={first}&second={second}", Json);

        Assert.NotNull(lca);
        Assert.Equal(expected, lca.Ancestor);
    }

    [Fact]
    public async Task Lca_MissingValue_ReturnsNotFound()
    {
        var client = await CreateClientWithDemoTreeAsync();

        var response = await client.GetAsync("/lca?first=1&second=999");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Tree_ReturnsNodesAndEdgesWithLayoutHints()
    {
        var client = await CreateClientWithDemoTreeAsync();

        var graph = await client.GetFromJsonAsync<Graph>("/tree", Json);

        Assert.NotNull(graph);
        Assert.Equal(9, graph.Count);
        Assert.Equal(4, graph.Height);
        Assert.Equal(9, graph.Nodes.Count);
        Assert.Equal(8, graph.Edges.Count); // n - 1 edges in a tree

        // Nodes carry in-order x-indexes 0..n-1 matching sorted order.
        var byXIndex = graph.Nodes.OrderBy(n => n.XIndex).Select(n => n.Value).ToArray();
        Assert.Equal(new[] { 1, 3, 4, 6, 7, 8, 10, 13, 14 }, byXIndex);

        // Root is at depth 0; children edges are parent -> child.
        var root = Assert.Single(graph.Nodes, n => n.Depth == 0);
        Assert.Equal(8, root.Value);
        Assert.Contains(graph.Edges, e => e.From == 8 && e.To == 3);
        Assert.Contains(graph.Edges, e => e.From == 8 && e.To == 10);
    }

    [Fact]
    public async Task Reset_ClearsTree()
    {
        var client = await CreateClientWithDemoTreeAsync();

        var reset = await client.PostAsync("/reset", content: null);
        Assert.Equal(HttpStatusCode.NoContent, reset.StatusCode);

        var graph = await client.GetFromJsonAsync<Graph>("/tree", Json);
        Assert.NotNull(graph);
        Assert.Equal(0, graph.Count);
        Assert.Empty(graph.Nodes);
    }

    [Fact]
    public async Task StaticVisualizer_IsServedAtRoot()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/");
        var html = await response.Content.ReadAsStringAsync();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Contains("Binary Tree Visualizer", html);
    }

    [Fact]
    public async Task SwaggerJson_IsAvailable()
    {
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        var response = await client.GetAsync("/swagger/v1/swagger.json");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var doc = await response.Content.ReadAsStringAsync();
        Assert.Contains("/traverse/{order}", doc);
        Assert.Contains("/lca", doc);
    }
}
