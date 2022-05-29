using Unity.Entities;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float moveSpeed;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<Moving>(entity);
        dstManager.AddComponentData(entity, new DestinationNode());
        dstManager.AddComponentData(entity, new InDistance { value = true });
        dstManager.AddComponentData(entity, new MoveSpeed { value = moveSpeed });
    }
}