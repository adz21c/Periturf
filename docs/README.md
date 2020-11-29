# Periturf

Library to manage the stubbing and mocking of environment components. 

## Quick Start

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
            w.Configure(wb => wb.UseUrls(WebHostUrl));
            w.WebApp("MyApp", "/MyApp");
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
                    r.Predicate(x => x.Request.Method.ToLower() == "get");
                    r.Response(rs =>
                    {
                        rs.StatusCode = HttpStatusCode.OK;
                        rs.ObjectBody(ob =>
                        {
                            ob.Object(new { Test = "Value" });
                            ob.JsonSerializer();
                        });
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
