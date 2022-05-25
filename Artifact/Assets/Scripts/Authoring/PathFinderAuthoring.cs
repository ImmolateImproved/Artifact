using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class PathFinderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new WaypointsMovement());

        dstManager.AddComponentData(entity, new PathfindingTarget());

        dstManager.AddBuffer<UnitPath>(entity);
    }
}