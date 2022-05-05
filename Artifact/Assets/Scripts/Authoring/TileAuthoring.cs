using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public class TileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int2 indexInGrid;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TileTag { });
        dstManager.AddComponentData(entity, new IndexInGrid
        {
            value = indexInGrid
        });
    }
}