using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AIAgentAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public AxialDirections initialDirection;
    public float moveSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<UnitTag>(entity);
        dstManager.AddComponent<StepCounters>(entity);
        dstManager.AddComponent<Moving>(entity);
        dstManager.AddComponent<MoveDestination>(entity);
        dstManager.AddComponent<IndexInGrid>(entity);
        dstManager.AddComponent<PreviousGridIndex>(entity);

        dstManager.AddComponentData(entity, new MoveDirection { value = initialDirection });
        dstManager.AddComponentData(entity, new MoveSpeed { value = moveSpeed });
    }
}