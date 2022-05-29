using Unity.Entities;
using UnityEngine;
using Unity.Physics.Authoring;
using Unity.Physics;
using Unity.Mathematics;
using NonGrid.Components;

public class SettingsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public PhysicsCategoryTags belongsTo;
    public PhysicsCategoryTags collidesWith;

    public float searchRadius;

    public float minUnitSpeed;
    public float maxUnitSpeed;

    public float2 boundings;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var collisionFilter = new CollisionFilter
        {
            BelongsTo = belongsTo.Value,
            CollidesWith = collidesWith.Value
        };

        dstManager.AddComponentData(entity, new AISettings
        {
            searchRadius = searchRadius,
            maxUnitSpeed = maxUnitSpeed,
            minUnitSpeed = minUnitSpeed,
            boundings = boundings,
            collisionFilter = collisionFilter
        });

    }
}