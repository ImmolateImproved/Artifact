using Unity.Entities;
using UnityEngine;

public class SelectionColorsAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public Color hoveredColor;
    public Color selectedColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SelectionColors
        {
            hoveredColor = hoveredColor,
            selectedColor = selectedColor

        });
    }
}