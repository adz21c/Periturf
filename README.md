# Periturf

Library to manage the stubbing and mocking of environment components. 

## Status

| Build         | [![Build status](https://ci.appveyor.com/api/projects/status/uwewgbfhrhul8jct/branch/master?svg=true)](https://ci.appveyor.com/project/adz21c/periturf/branch/master) |
| Code Coverage | [![Coverage](https://sonarcloud.io/api/project_badges/measure?branch=master&project=adz21c_Periturf&metric=coverage)](https://sonarcloud.io/dashboard?id=adz21c_Periturf&branch=master) [![codecov](https://codecov.io/gh/adz21c/Periturf/branch/master/graph/badge.svg)](https://codecov.io/gh/adz21c/Periturf/branch/master) |
| Metrics       | [![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?branch=master&project=adz21c_Periturf&metric=alert_status)](https://sonarcloud.io/dashboard?id=adz21c_Periturf&branch=master) |

## NuGet packages

| Periturf         | [![Nuget](https://img.shields.io/nuget/v/periturf.svg)](https://www.nuget.org/packages/Periturf/) |
| Periturf.Hosting | [![Nuget](https://img.shields.io/nuget/v/periturf.hosting.svg)](https://www.nuget.org/packages/Periturf.Hosting/) |

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
