# Quick Start

Add a package reference to [Periturf](https://www.nuget.org/packages/Periturf).

```powershell
dotnet add package Periturf
```

Add a package reference to a Periturf Component. For example [Periturf.Web](https://www.nuget.org/packages/Periturf.Web).

```powershell
dotnet add package Periturf.Web
```

Create an Environment and start it.

```csharp
var env = Environment.Setup(e =>
{
    e.GenericHost(h =>
    {
        h.Web(w =>
        {
            w.WebApp("MyApp", "/MyApp");
            w.BindToUrl(WebHostUrl);
        });
    });
});

await env.StartAsync();
```

Configure the component's behaviour and remove the behaviour on tear down (example using NUnit).

```csharp
[TestFixture]
class WebTests
{
    private IDisposable _envConfiguration;

    [SetUp]
    public Task SetUp()
    {
        _envConfiguration = await env.ConfigureAsync(c =>
        {
            // All Get Requests return the same thing in json
            c.WebApp("MyApp", w =>
            {
                w.OnRequest(r =>
                {
                    r.Criteria(x => x.Method().EqualTo("GET"));
                    r.Response(rs =>
                    {
                        rs.StatusCode(200);
                        rs.Body(ob => ob.Content(new { Test = "Value" }));
                    });
                });
            });
        });
    }

    [TearDown]
    public Task TearDown()
    {
        await _envConfiguration.DisposeAsync();
    }

    [Test]
    public async Task Test()
    {
        var client = new HttpClient() { BaseAddress = new Uri(BasePath) };

        // 404 for post
        var postResponse = await client.PostAsync(BasePath, new StringContent(""));
        Assert.That(postResponse.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));

        // 200 for get
        var getResponse = await client.GetAsync(BasePath);
        Assert.That(getResponse.StatusCode, Is.EqualTo(HttpStatusCode.OK));
    }
}
```
