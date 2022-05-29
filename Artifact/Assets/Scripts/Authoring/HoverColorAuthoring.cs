using Unity.Entities;
using UnityEngine;

public class HoverColorAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color hoverColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new HoverColor
        {
            value = hoverColor

        });
    }
}