# Using Periturf

## Add as dependency

Add a package reference to [Periturf](https://www.nuget.org/packages/Periturf).

```powershell
dotnet add package Periturf
```

Add a package reference to a Periturf Component. For example [Periturf.Web](https://www.nuget.org/packages/Periturf.Web).

```powershell
dotnet add package Periturf.Web
```

## Setup

An *environment* is the central entity through which any alterations of mock services will happen. Create an *environment* by calling the static *setup* method.

```csharp
var env = Environment.Setup(s => { });
```

During *setup* you define each mock service you require, these are referred to as *components*. These components run on *Hosts*, which are the entities that an *environment* will *start* to bring the mock services to life and *stop* to remove them. In the below example we create a WebApp *component* running on a generic host.

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
```

*Hosts* can run multiple *components*, whether you should have a *host* per *component* or multiple *components* per *host* will come down to the needs of individual *components* or your use case.

Finally, before doing anything else with an *environment* you must *start* it.

```csharp
await env.StartAsync();
```

## Configure

Modifying the behaviour of a component is achieved by configuring your environment. In the below example we *configure* the MyApp WebApp component to return a 200 response code with an object serialized to JSON for all GET requests.

```csharp
var handle = await env.ConfigureAsync(e =>
{
    e.WebApp("MyApp", w =>
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
```

Configuring will return a disposable *configuration handle*. Disposing of this *handle* will remove the configuration from the environment.

```csharp
await handle.DisposeAsync();

await using (await env.ConfigureAsync(e => { }))
{
    // Do something
}
```

You can *configure* multiple components at the same time and disposing of the *handle* will remove the configuration from all the components at the same time.

```csharp
var handle = await env.ConfigureAsync(e =>
{
    e.WebApp("MyApp", w =>
    {
        // Configure
    });

    e.WebApp("MyOtherApp", w =>
    {
        // Configure
    });
});

await handle.DisposeAsync();
```

You can *configure* multiple times and build up multiple *handles* that can be disposed separately. There is no rule as to how components will behave with multiple configuration or the order of disposal. The individual components will determine how to best handle this condition, so refer to their documentation. However, a good default is to think of the configuration as a stack of filters ordered with the most recent on top and any interactions fall through until they hit a filter that is relevant. For example:

```csharp
// GET 404
// POST 404

var get200 = await env.ConfigureAsync(e => e.WebApp("MyApp", w => { }));
// GET 200
// POST 404

var post200 = await env.ConfigureAsync(e => e.WebApp("MyApp", w => { }));
// GET 200
// POST 200

var post204 = await env.ConfigureAsync(e => e.WebApp("MyApp", w => { }));
// GET 200
// POST 204

await get200.DisposeAsync();
// GET 404
// POST 204

await post200.DisposeAsync();
// GET 404
// POST 204

await post204.DisposeAsync();
// GET 404
// POST 404
```

## Discussion

Envrionments can be setup and discarded as you wish, but Periturf has been designed expecting an environment to be a long running dependency of your System Under Test (SUT). Immediately after setting up your environment you will likely want to configure some default behaviour that is common for all tests, but configuration can be created and disposed per-test (or groups of tests). Depending on the purpose and implementation of your SUT this enables you to have a single running instance of your SUT, while executing multiple concurrent tests against it.

### NUnit Example

In the below example our SUT will be a function that checks a URL exists or not (for simplicity, in real life the SUT could be a much more complex beast). Here we create an environment that is shared by two test fixtures.

```csharp
[TestFixtureSetUp]
public class Environment
{
    private static IDisposable _envConfiguration;

    public static Environment Instance { get; private set; }

    [OneTimeSetUp]
    public Task SetUp()
    {
        Instance = Environment.Setup(e =>
        {
            e.GenericHost(h =>
            {
                h.WebApp(w =>
                {
                    w.Configure(wb => wb.UseUrls("http://localhost:8080"));
                    w.WebApp("MyApp", "/MyApp");
                });
            });
        });

        await Instance.StartAsync();

        await _envConfiguration = Instance.ConfigureAsync(c =>
        {
            c.WebApp(w =>
            {
                w.OnRequest(r =>
                {
                    r.Predicate(x => x.Request.Path == "/Path1");
                    r.Response(rs => rs.StatusCode = HttpStatusCode.OK);
                });
            });
        });
    }

    [OneTimeTearDown]
    public Task TearDown()
    {
        await _envConfiguration.DisposeAsync();
        await Instance.StopAsync();
    }
}

[TestFixture]
public class WithPath2Tests
{
    private IDisposable _config;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _config Environment.Instance.ConfigureAsync(c =>
        {
            c.WebApp(w =>
            {
                w.OnRequest(r =>
                {
                    r.Predicate(x => x.Request.Path == "/Path2");
                    r.Response(rs => rs.StatusCode = HttpStatusCode.OK);
                });
            });
        });
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _config.DisposeAsync();
    }

    [Test]
    public async Task Path1()
    {
        var result = MyApp.UrlTester("http://localhost:8080/Path1");
        Assert.That(result, Is.True);
    } 

    [Test]
    public async Task Path2()
    {
        var result = MyApp.UrlTester("http://localhost:8080/Path2");
        Assert.That(result, Is.True);
    }

    [Test]
    public async Task Path4()
    {
        var result = MyApp.UrlTester("http://localhost:8080/Path4");
        Assert.That(result, Is.False);
    }
}

[TestFixture]
public class WithPath3Tests
{
    private IDisposable _config;

    [OneTimeSetUp]
    public async Task SetUp()
    {
        _config Environment.Instance.ConfigureAsync(c =>
        {
            c.WebApp(w =>
            {
                w.OnRequest(r =>
                {
                    r.Predicate(x => x.Request.Path == "/Path3");
                    r.Response(rs => rs.StatusCode = HttpStatusCode.OK);
                });
            });
        });
    }
    
    [OneTimeTearDown]
    public async Task TearDown()
    {
        await _config.DisposeAsync();
    }

    [Test]
    public async Task Path1()
    {
        var result = MyApp.UrlTester("http://localhost:8080/Path1");
        Assert.That(result, Is.True);
    } 

    [Test]
    public async Task Path3()
    {
        var result = MyApp.UrlTester("http://localhost:8080/Path3");
        Assert.That(result, Is.True);
    } 

    [Test]
    public async Task Path5()
    {
        var result = MyApp.UrlTester("http://localhost:8080/Path5");
        Assert.That(result, Is.False);
    }
}
```

This example demonstrates shared configuration and configuration for individual test fixtures. Configuring the environment as part of test setup keeps dependencies close and makes test code easier to manage. However, shared configuration also saves lots of duplicate code and execution.
