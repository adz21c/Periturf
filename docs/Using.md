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
            w.WebApp("MyApp", "/MyApp");
            w.BindToUrl(WebHostUrl);
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
            r.Criteria(x => x.Method().EqualTo("GET"));
            r.Response(rs =>
            {
                rs.StatusCode(200);
                rs.Body(ob => ob.Content(new { Test = "Value" }));
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

## Verify

You can test that your system under test interfacts with its environment by declaring verifications. A verification is made up of:
- **Conditions** - Registered with the component defining an event of interest.
  - Produces a feed of *condition instances* that indicate when the condition was met.
- An **Expectation** - That defines a set of constraints that all must be met and evaluates it against the feed of *condition instances*.
- **Constraints** - Defines an event that must happen to meet the expectation with optional additional restrictions.

```csharp
var verifier = env.VerifyAsync(ctx =>
{
    var conditionIdentifier = ctx.Condition(c => c.WebApp().OnRequest(...));
    ctx.Expect(e =>
    {
        e.Constraint(c => c.Condition(conditionIdentifier).Before(TimeSpan.FromSeconds(5))));
        e.Then(c => c.Condition(conditionIdentifier))));
    });
});
var result = await verifier.VerifyAsync();

if (!result.AsExpected)
    throw new Exception();
```

The above example verifies two web requests meeting the same condition. The first must happen within 5 seconds of initiating verification and the second anytime after.

All verifications are subject to an *inactivity timeout* which fails the verification if none of the registered conditions provide any relevant activity for the configured timeout. If the expectation constraints have a relevant timeout configured then the *inactivity timeout* is ignored until they are no-longer relevant. In the above the first request timeout ignores the *inactivity timeout*, but it becomes relevant for the second request. If the first request fails to happen within 5 seconds then the constraint timeout will fail the test. The *inactivity timeout* can be configured on environment setup or on a per-verification basis.

### Ordered and Unordered constraints

The below example shows a verification that requires condition 1 and 2 be met before condition 3 and 4. The order condition 1 and 2 are met is undefined and the order of condition 3 and 4 is also undefined.

```csharp
var verifier = env.VerifyAsync(ctx => ctx.Expect(e =>
{
    e.Constraint(c => c.Condition(ctx.Condition(c => c.WebApp().OnRequest(...))));  // Condition 1
    e.Constraint(c => c.Condition(ctx.Condition(c => c.WebApp().OnRequest(...))));  // Condition 2
    e.Then(c =>
    {
        c.Condition(ctx.Condition(c => c.WebApp().OnRequest(...)));  // Condition 3
        c.Condition(ctx.Condition(c => c.WebApp().OnRequest(...)));  // Condition 4
    });
}));
```

Constraints can be restricted by time, so you can define order without using *Then*. Either method works, but using time constraints might not be practical. *Then* allows you to define order without specifying time expectations.

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
                    w.WebApp("MyApp", "/MyApp");
                    w.BindToUrl("http://localhost:8080");
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
                    r.Criteria(x => x.Path().EqualTo("/Path1"));
                    r.Response(rs => rs.Status(200));
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
                    r.Criteria(x => x.Path().EqualTo("/Path2"));
                    r.Response(rs => rs.Status(200));
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
                    r.Criteria(x => x.Path().EqualTo("/Path3"));
                    r.Response(rs => rs.Status(200));
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
