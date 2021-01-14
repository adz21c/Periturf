using System.Collections.Generic;

namespace Periturf.Components
{
    class ComponentLocator
    {
        private readonly IDictionary<string, IComponent> _components;

        public ComponentLocator(IDictionary<string, IComponent> components)
        {
            _components = components;
        }

        public IComponent GetComponent(string name)
        {
            if (!_components.TryGetValue(name, out var component))
                throw new ComponentLocationFailedException(name);

            return component;
        }
    }
}
