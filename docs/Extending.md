
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

        public InternalObject OptionalFactory()
        {
            return new InternalObject(MyProperty);
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
            var spec = builder.CreateComponentConfigSpecification<MySpecification>(name);
            config.Invoke(spec);
            builder.AddSpecification(spec);
        }
    }
}
```

### Extensionable Extensions

It might be your extension can also have its behaviour altered through further extension. One way to achieve this is to repeat the fluent configuration convention on top of itself. Modify your *configurator* to accept *specification* interfaces. From there other extensions can repeat the convention on top of your code.

Here we can see MyExtension has changed to also take in specifications and construct its "InternalObject" using components provided by those specifications.

```csharp
namespace Periturf.MyExtension
{
    class MySpecification : IMyConfigurator
    {
        private readonly List<IOtherSpecification> _specs = new List<IOtherSpecification>();

        public string MyProperty { get; set; }

        public void AddOtherSpecification(IOtherSpecification spec)
        {
            _spec.Add(spec);
        }

        public InternalObject OptionalFactory()
        {
            return new InternalObject(
                MyProperty
                _specs.Select(x => x.OtherFactory()).ToList());
        }
    }

    public interface IMyConfigurator
    {
        string MyProperty { get; set; }

        void AddOtherSpecification(IOtherSpecification spec);
    }

    public interface IOtherSpecification
    {
        OtherObject OtherFactory();
    }
}
```

OtherExtension looks almost the same as MyExtension in the earlier example.

```csharp
namespace Periturf.OtherExtension
{
    public interface IOtherConfigurator
    {
        string OtherProperty { get; set ;}
    }

    class OtherSpecification : IOtherSpecification, IOtherConfigurator
    {
        public string OtherProperty { get; set; }

        public OtherObject OtherFactory()
        {
            return new OtherObject(OtherProperty);
        }
    }
}
```

MyExtension *entry point* remains unchanged, but the OtherExtension *entry point* is initiated off the MyExtension *configurator*.

```csharp
namespace Periturf
{
    public static class OtherExtension
    {
        public static void OtherExtension(this IMyConfigurator configurator, Action<IOtherConfigurator> config)
        {
            var spec = new OtherSpecification();
            config.Invoke(spec);
            configurator.AddOtherSpecification(spec);
        }
    }
}
```

## Setup

Hosts extend *IHost* and will need to store a dictionary of *IComponents* with the key as the component name. The host is created by implementing *IHostSpecification* as a factory for the host. Following the previously discussed Fluent Configuration you will need to create a specification interface for components that others will extend.

Components extend the *IComponent* interface. The specification interface required will depend on the implementation of the host it will sit on top of.

## Configuration

New configuration is created in the form of a class implementing *IConfigurationSpecification*. The configuration specification will be created via *IConfigurationContext* that will ultimately call your component to create the specification. This gives you an opportunity to inject any dependencies into your specification. The *IEventHandlerFactory* is available at this point that allows you to hook into some generic event handling. Once the specification has been configured it will need registering with *IConfigurationContext.AddSpecification*. Using the previously discussed Fluent Configuration the *entry point* will take care of all of this and expose the configuration as a *configurator* interface.

```csharp
namespace Periturf.MyExtension
{
    class ConfigurationSpecification : IConfigurationSpecification, IMyExtensionConfigurator
    {
        // ...
    }

    class MyExtensionComponent : IComponent
    {
        // ...

        public TSpecification CreateConfigurationSpecification<TSpecification>(IEventHandlerFactory eventHandlerFactory) where TSpecification : IConfigurationSpecification
        {
            return (TSpecification)(object)new ConfigurationSpecification(eventHandlerFactory);
        }
    }
}

namespace Periturf
{
    public static class MyExtension
    {
        public static void MyExtension(this IConfigurationContext builder, string name, Action<IMyExtensionConfigurator> config)
        {
            var spec = builder.CreateComponentConfigSpecification<ConfigurationSpecification>(name);
            config.Invoke(spec);
            builder.AddSpecification(spec);
        }
    }
}
```

Once your specification is created, configured, and registered then Periturf will call your specification to apply configuration. This will call the specification's *ApplyAsync* method, which will return a *IConfigurationHandle*. The configuration handle is *IDisposableAsync* that when called will rollback the configuration. 

```csharp
namespace Periturf.MyExtension
{
    class ConfigurationSpecification : IConfigurationSpecification, IMyExtensionConfigurator
    {
        public Task ApplyAsync(CancellationToken ct)
        {
            // do things
            return new ConfigurationHandle(dependency);
        }
    }

    class ConfigurationHandle : IConfigurationHandle
    {
        // ...
    }
}
```

## Verification

Components provide conditions that can be verified. Components are created via the *IConditionSpecification* that will create an *IConditionFeed*. The *IConditionConfigurator* provides a method to retrieve an *IConditionBuilder* from your component. The component builder will need to be casted to your implementation that will expose your *IConditionSpecification* factory methods. Unlike other areas where the specification is registered using an *AddSpecification* method on the appropriate configurator, in this case we return the specification. This is purely for the code asthetics, to remove excessive nesting.

```csharp
namespace Periturf.MyExtension
{
    class ConditionSpecification : IConditionSpecification
    {
        // ...
    }

    public class ConditionBuilder : IConditionBuilder
    {
        public ConditionSpecification MyCondtion()
        {
            // ...
        }
    }

    class MyExtensionComponent : IComponent
    {
        // ...

        public IConditionBuilder CreateConditionBuilder()
        {
            return new ConditionBuilder();
        }
    }
}

namespace Periturf
{
    public static class MyExtension
    {
        public static ConditionBuilder MyExtension(this IConditionConfigurator configurator, string name)
        {
            return (ConditionBuilder) configurator.GetConditionBuilder(name);
        }
    }
}
```

*IConditionSpecification* will be called to create the *IConditionFeed*. This should register the condition with the component and start gathering *ConditionInstances*. *WaitForInstances* will be called on the *IConditionFeed* which will wait until it receives instances to pass on. When the verification is done the provided CancellationToken will be cancelled. *IConditionFeed* will be disposed when the verifier has no need for the condition, which should unregister the condition with the component.

## Discussion

Configuration and Verification both aim to identify an event within your component to react and record respectively. Therefore, it makes sense to re-use whatever configuration code you can between these two areas to enable users to also write code that can be shared.
