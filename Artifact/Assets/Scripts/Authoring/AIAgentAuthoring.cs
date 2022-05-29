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


        dstManager.AddComponentData(entity, new MoveDirection { value = initialDirection });

    }
}