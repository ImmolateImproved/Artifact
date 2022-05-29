using Unity.Entities;
using UnityEngine;
using NonGrid.Components;

public class BugAIAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float moveSpeed;
    public float rotationSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveSpeed { value = moveSpeed });
        dstManager.AddComponentData(entity, new RotationChangeData
        {
            speed = rotationSpeed
        });

        dstManager.AddComponentData(entity, new UnitTag());

        dstManager.AddComponentData(entity, new AIData { target = ObjectTypes.Resource });
        dstManager.AddComponentData(entity, new NotificationListener());
        dstManager.AddComponentData(entity, new NeighborObject());

        dstManager.AddBuffer<NeighborUnit>(entity);
    }
}