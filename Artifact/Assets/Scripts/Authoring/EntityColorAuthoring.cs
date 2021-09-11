using Unity.Entities;
using UnityEngine;

public class EntityColorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color defaultColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new EntityColors { defaultColor = defaultColor });
    }
}