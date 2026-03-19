using System.Collections.Generic;

namespace EntityComponentSystem;

public class EntityManager {
    private Dictionary<ulong, Entity> _entities;

    public void AddEntity(Entity entity) {
        _entities[entity.EntityId] = entity;
    }

    public void RemoveEntity(ulong entityId) {
        _entities.Remove(entityId);
    }

    public Entity GetEntity(ulong entityId) {
        return _entities[entityId];
    }
}