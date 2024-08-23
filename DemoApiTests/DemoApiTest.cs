using DemoKiotaClientHandler;
using KiotaPosts.Client;
using KiotaPosts.Client.Models;
using Microsoft.Kiota.Http.HttpClientLibrary.Middleware.Options;

namespace DemoApiTests;

[TestFixture, Description("Kiota Clients with Nunit POC")]
public class DemoApiTest
{
    PostsClient postsClient;

    [SetUp]
    public void Setup()
    {
        postsClient = DemoKiotaClientManager.GetDemoApiClient();
    }

    [Test, Description("Get all posts")]
    public async Task TestGetAllPosts()
    {
        var allPosts = await postsClient.Posts.GetAsync();
        Assert.That(allPosts?.Count, Is.EqualTo(100));
        foreach (var post in allPosts!)
        {
            Assert.Multiple(() =>
            {
                Assert.That(post.AdditionalData, Is.Not.Null);
                Assert.That(post.Body, Is.Not.Null);
                Assert.That(post.Id, Is.Not.Null);
                Assert.That(post.Title, Is.Not.Null);
                Assert.That(post.UserId, Is.Not.Null);
            });
        }
    }

    [Description("Get posts by ID")]
    [TestCase(1)]
    [TestCase(20)]
    [TestCase(30)]
    public async Task TestGetPostById(int id)
    {
        var post = await postsClient.Posts[id].GetAsync();

        Assert.Multiple(() =>
        {
            Assert.That(post?.AdditionalData, Is.Not.Null);
            Assert.That(post?.Body, Is.Not.Null);
            Assert.That(post?.Id, Is.Not.Null);
            Assert.That(post?.Id, Is.EqualTo(id));
            Assert.That(post?.Title, Is.Not.Null);
            Assert.That(post?.UserId, Is.Not.Null);
        });
    }

    [Description("Create new post")]
    [Test]
    public async Task TestCreatePost()
    {
        var rnd = new Random();
        var id = rnd.Next(100, 1000);

        var newPost = new Post
        {
            UserId = id,
            Title = $"Testing Kiota-generated API client - id {id}",
            Body = $"Hello {id}"
        };

        var createdPost = await postsClient.Posts.PostAsync(newPost);

        Assert.Multiple(() =>
        {
            Assert.That(createdPost?.UserId, Is.EqualTo(id));
        });
    }

    [Description("Patch post")]
    [TestCase(1, "Updated Title 1")]
    [TestCase(20, "Updated Title 20")]
    [TestCase(30, "Updated Title 30")]
    public async Task TestPatchPost(int id, string updatedTitle)
    {
        var update = new Post
        {
            Title = updatedTitle
        };

        var updatedPost = await postsClient.Posts[id].PatchAsync(update);

        Assert.Multiple(() =>
        {
            Assert.That(updatedPost?.Id, Is.EqualTo(id));
            Assert.That(updatedPost?.Title, Is.EqualTo(updatedTitle));
        });
    }

    [Description("Delete Post")]
    [TestCase(1)]
    [TestCase(20)]
    [TestCase(30)]
    public async Task DeletePost(int id)
    {
        var observeOptions = new HeadersInspectionHandlerOption
        {
            InspectRequestHeaders = true,
            InspectResponseHeaders = true,
        };

        var deletedPostResponse = await postsClient.Posts[id].DeleteAsync(x => x.Options.Add(observeOptions));

        Assert.That(deletedPostResponse, Is.Not.Null);
    }
}