
# Extending Periturf

Periturf is not very interesting by itself, but is useful as a base for other tools.

## Fluent Configuration

While not required, a consistent approach to configuration will lower the learning curve when configuring your extension. The conventions defined here should also help with discoverablity of your configuration, through intellisense.

**Specification** class
The specification class works as both a container for the configuration and a factory for whatever internal components are necessary to apply the configuration.

**Configurator** interface
This is the interface exposed to the user while configuring the environment and is implemented by the *specification*.

**Entry point** extension method
The entry point is an extension method that applies the configuration. The method is made discoverable by putting it in the Periturf namespace. Depending on the configuration complexity the *entry point* can accept parameters for configuration and/or an action method that takes a *configurator* parameter to apply configuration to a *specification*.

Bringing these elements all together:

```csharp
namespace Periturf.MyExtension
{
    class MySpecification : IMyConfigurator
    {
        public string MyProperty { get; set; }

        public Object OptionalFactory()
        {
            // do things
        }
    }

    public interface IMyConfigurator
    {
        string MyProperty { get; set; }
    }
}

namespace Periturf
{
    public static class MyExtension
    {
        public static void MyExtension(this IConfigurationContext builder, string name, Action<IMyConfigurator> config)
        {
            var spec = builder.CreateComponentConfigSpecification<WebComponentSpecification>(name);
            config.Invoke(spec);
            builder.AddSpecification(spec);
        }
    }
}
```

### Extensionable Extensions

It might be your extension can also work as a base to more components or have its behaviour further altered. One way to achieve this is to repeat the fluent configuration convention on top of itself. Modify your *configurator* to accept *specification* interfaces. From there other extensions can repeat the convention on top of your code. For example:

```csharp
namespace Periturf.MyExtension
{
    class MySpecification : IMyConfigurator
    {
        private readonly List<IOtherSpecification> _specs = new List<IOtherSpecification>();

        public string MyProperty { get; set; }

        public void AddOtherSpecification

        public Object OptionalFactory()
        {
            // do things
        }
    }

    public interface IMyConfigurator
    {
        string MyProperty { get; set; }
    }
}

namespace Periturf
{
    public static class MyExtension
    {
        public static void MyExtension(this IConfigurationContext builder, string name, Action<IMyConfigurator> config)
        {
            var spec = builder.CreateComponentConfigSpecification<WebComponentSpecification>(name);
            config.Invoke(spec);
            builder.AddSpecification(spec);
        }
    }
}
```
