using Unity.Entities;
using UnityEngine;

public class SelectionAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color hoverColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SelectedUnit());
        dstManager.AddComponentData(entity, new HoverTile());

        dstManager.AddComponentData(entity, new HoverColor
        {
            value = hoverColor

        });
    }
}