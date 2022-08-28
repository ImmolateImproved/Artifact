using Unity.Entities;
using UnityEngine;

public class MovementAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public float moveSpeed;
    public int moveRange;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveTarget());
        dstManager.AddComponentData(entity, new MoveSpeed { value = moveSpeed });
        dstManager.AddComponentData(entity, new MovementRange { value = moveRange });
    }
}