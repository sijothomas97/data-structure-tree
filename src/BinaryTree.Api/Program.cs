using BinaryTree.Api;
using CoreTree = BinaryTree.Core.BinaryTree<int>;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BinaryTree API",
        Version = "v1",
        Description = "Build a binary search tree of integers and inspect it: "
            + "insert values, run traversals, query lowest common ancestors, "
            + "and fetch the whole tree as a node/edge graph for visualization.",
    });
});
builder.Services.AddSingleton<TreeStore>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();

app.MapPost("/insert", (InsertRequest request, TreeStore store) =>
    {
        var response = store.With(tree =>
        {
            bool added = tree.Insert(request.Value);
            return new InsertResponse(request.Value, added, tree.Count);
        });
        return Results.Ok(response);
    })
    .WithName("Insert")
    .WithDescription("Inserts a value into the tree. Duplicates are rejected (added = false).")
    .Produces<InsertResponse>();

app.MapPost("/reset", (TreeStore store) =>
    {
        store.Reset();
        return Results.NoContent();
    })
    .WithName("Reset")
    .WithDescription("Clears the tree.");

app.MapGet("/traverse/{order}", (string order, TreeStore store) =>
    {
        var sequence = store.With<IReadOnlyList<int>?>(tree => order.ToLowerInvariant() switch
        {
            "inorder" or "in-order" or "in" => tree.InOrder().ToList(),
            "preorder" or "pre-order" or "pre" => tree.PreOrder().ToList(),
            "postorder" or "post-order" or "post" => tree.PostOrder().ToList(),
            _ => null,
        });

        return sequence is null
            ? Results.BadRequest(new { error = $"Unknown traversal order '{order}'. Use inorder, preorder, or postorder." })
            : Results.Ok(new TraversalResponse(order.ToLowerInvariant(), sequence));
    })
    .WithName("Traverse")
    .WithDescription("Returns the visit sequence for the given traversal order (inorder | preorder | postorder).")
    .Produces<TraversalResponse>()
    .Produces(StatusCodes.Status400BadRequest);

app.MapGet("/lca", (int first, int second, TreeStore store) =>
    {
        var ancestor = store.With(tree => tree.FindLowestCommonAncestor(first, second));
        return ancestor is null
            ? Results.NotFound(new { error = "Both values must exist in the tree." })
            : Results.Ok(new LcaResponse(first, second, ancestor.Value));
    })
    .WithName("LowestCommonAncestor")
    .WithDescription("Returns the lowest common ancestor of two stored values, e.g. /lca?first=3&second=11.")
    .Produces<LcaResponse>()
    .Produces(StatusCodes.Status404NotFound);

app.MapGet("/tree", (TreeStore store) =>
    {
        var graph = store.With(BuildGraph);
        return Results.Ok(graph);
    })
    .WithName("GetTree")
    .WithDescription("Returns the whole tree as nodes (with in-order x-index and depth layout hints) and parent-child edges.")
    .Produces<TreeGraphResponse>();

app.Run();

static TreeGraphResponse BuildGraph(CoreTree tree)
{
    var nodes = new List<NodeDto>();
    var edges = new List<EdgeDto>();
    int xIndex = 0;

    void Walk(CoreTree.Node? node, int depth)
    {
        if (node is null)
        {
            return;
        }

        Walk(node.Left, depth + 1);
        nodes.Add(new NodeDto(node.Value, node.Value, xIndex++, depth));
        Walk(node.Right, depth + 1);

        if (node.Left is not null)
        {
            edges.Add(new EdgeDto(node.Value, node.Left.Value));
        }
        if (node.Right is not null)
        {
            edges.Add(new EdgeDto(node.Value, node.Right.Value));
        }
    }

    Walk(tree.Root, 0);
    return new TreeGraphResponse(nodes, edges, tree.Count, tree.Height());
}

/// <summary>Marker so WebApplicationFactory-based integration tests can target the API.</summary>
public partial class Program
{
}
