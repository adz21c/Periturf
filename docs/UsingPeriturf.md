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

An *Environment* is the central entity through which any alterations of mock services will happen. Create an *Environment* by calling the static *Setup* method.

```csharp
var env = Environment.Setup(s => { });
```

During *Setup* you define each mock service you require, these are referred to as *Components*. These components run on *Hosts*, which are the entities that an *Environment* will *Start* to bring the mock services to life and *Stop* to remove them. In the below example we create a WebApp *Component* running on a generic host.

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

*Hosts* can run multiple *Components*, whether you should have a *Host* per *Component* or multiple *Components* per *Host* will come down to the needs of individual *Components* or your use case.

Finally, before doing anything else with an *Environment* you must *Start* it.

```csharp
await env.StartAsync();
```

## Configure

Modifying the behaviour of a *Component* is achieved by *Configuring* your *Environment*. In the below example we configure the MyApp WebApp component to return a 200 response code with an object serialized to JSON for all GET requests.

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

Configuring will return a disposable handle for that configuration. Disposing of this handle will remove the configuration from the environment.

```csharp
await handle.DisposeAsync();

await using (await env.ConfigureAsync(e => { }))
{
    // Do something
}
```

You can configure multiple components at the same time and disposing of the handle will remove the configuration from all the components at the same time.

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

You can call configuration multiple times and build up multiple handles that can be disposed separately. There is no rule as to how components will behave with multiple configuration or the order of disposal. The individual components will determine how to best handle this condition, so refer to their documentation. However, a good default is to think of the configuration as a stack of filters ordered with the most recent on top and any interactions fall through until they hit a filter that is relevant. For example:

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
