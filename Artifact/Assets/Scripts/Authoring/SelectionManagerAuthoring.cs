using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Authoring;
using UnityEngine;

public class SelectionManagerAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public CollisionFilter collisionFilter;

    public PhysicsCategoryTags belongsTo;
    public PhysicsCategoryTags collidesWith;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MousePosition());

        var collisionFilter = new CollisionFilter
        {
            BelongsTo = belongsTo.Value,
            CollidesWith = collidesWith.Value
        };

        dstManager.AddComponentData(entity, new SelectionManager
        {
            collisionFilter = collisionFilter

        });
    }
}