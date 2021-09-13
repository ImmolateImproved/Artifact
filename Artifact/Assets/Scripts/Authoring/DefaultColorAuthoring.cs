using Unity.Entities;
using UnityEngine;

public class DefaultColorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color defaultColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DefaultColor { defaultColor = defaultColor });
    }
}