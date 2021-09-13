using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PathFinderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float moveSpeed;
    public bool addMovementComponents;

    public bool drawPath;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (drawPath)
        {
            dstManager.AddComponentData(entity, new DrawPath());
        }

        if (addMovementComponents)
        {
            dstManager.AddComponentData(entity, new WaypointsMovement());
            dstManager.AddComponentData(entity, new MoveSpeed { value = moveSpeed });
        }

        dstManager.AddComponentData(entity, new PathfindingTarget());

        dstManager.AddBuffer<UnitPath>(entity);
    }
}