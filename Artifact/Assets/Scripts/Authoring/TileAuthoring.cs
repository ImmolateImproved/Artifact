using Unity.Entities;
using UnityEngine;

public class TileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TileTag { });
        dstManager.AddComponentData(entity, new IndexInGrid { });
    }
}