using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequireComponent(typeof(MovementAuthoring))]
public class AIAgentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public AxialDirections initialDirection;
    public int notificationRange;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<NotificationListener>(entity);
        dstManager.AddComponent<Moving>(entity);

        dstManager.AddComponentData(entity, new MoveDirection { value = initialDirection });

        dstManager.AddComponentData(entity, new SwarmIntelligenceData
        {
            target = GridObjectTypes.Base
        });

        dstManager.AddComponentData(entity, new NotificationRange
        {
            value = notificationRange
        });

        dstManager.AddBuffer<NeighborGridObjects>(entity);
    }
}