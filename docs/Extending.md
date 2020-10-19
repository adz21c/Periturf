
# Extending Periturf

Periturf is not very interesting by itself, but is useful as a base for other tools.

## Fluent Configuration

While not required, a consistent approach to configuration will lower the learning curve when configuring your extension. The conventions defined here should also help with discoverablity of your configuration, through intellisense.

**Specification** class
The specification class works as both a container for the configuration and a factory for whatever internal components are necessary to apply the configuration.

```csharp
namespace Periturf.MyExtension
{
    class MySpecification : IMyConfigurator
    {
        public string MyProperty { get; set; }
    }
}
```

**Configurator** interface
This is the interface exposed to the user while configuring the environment and is implemented by the *specification*.

```csharp
namespace Periturf.MyExtension
{
    public interface IMyConfigurator
    {
        string MyProperty { get; set; }
    }
}
```

**Entry point** extension method
The entry point is an extension method that applies the configuration. The method is made discoverable by putting it in the Periturf namespace. Depending on the configuration complexity the *entry point* can accept parameters for configuration and/or an action method that takes a *configurator* parameter to apply configuration to a *specification*.

```csharp
namespace Periturf.MyExtension
{
    public interface IMyConfigurator
    {
        string MyProperty { get; set; }
    }
}
```
