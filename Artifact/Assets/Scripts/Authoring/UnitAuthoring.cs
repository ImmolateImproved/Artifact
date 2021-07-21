using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int moveRange;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var indexInGrid = (int2)(((float3)transform.position).xz + 0.5f);

        dstManager.AddComponentData(entity, new MoveRange { value = moveRange });

        dstManager.AddComponentData(entity, new IndexInGrid
        {
            value = indexInGrid
        });

        dstManager.AddComponentData(entity, new PreviousGridIndex
        {
            value = indexInGrid
        });

        dstManager.AddComponent<Selectable>(entity);
    }
}