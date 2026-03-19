using System;
using System.Collections.Generic;

namespace EntityComponentSystem;

public class Entity {
    public ulong EntityId { get; }

    private readonly Dictionary<Type, IComponent> _components = [];

    public Entity(ulong entityId) {
        EntityId = entityId;
    }

    public void AddComponent(Type type, IComponent component) {
        _components[type] = component;
    }

    public bool HasComponent(Type componentType) {
        return _components.ContainsKey(componentType);
    }

    public T GetComponent<T>() where T : IComponent {
        if (_components.TryGetValue(typeof(T), out IComponent component)) {
            return (T)component;
        }

        return default;
    }
}