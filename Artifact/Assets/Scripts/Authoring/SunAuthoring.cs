using Unity.Entities;
using UnityEngine;

public class SunAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int range;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Sun { range = range });
        dstManager.AddComponentData(entity, new IndexInGrid { });
    }
}