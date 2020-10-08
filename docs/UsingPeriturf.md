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

## Setup an Environment

*Environments* are the central elements through which any alterations of mock services will happen. Create an *Environment* by calling the static *Setup* method.

```csharp
var env = Environment.Setup(s => { });
```

During *Setup* you define each mock service you require, these are referred to as *Components*. These components run on *Hosts*, which are the elements that an *Environment* will *Start* to bring the mock services to life and *Stop* to remove them. In the below example we create a WebApp *Component* running on a generic host.

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
