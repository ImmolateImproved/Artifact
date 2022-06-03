using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class SelectionManagerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject selectionPointer;
    public float selectionPointerYPosition;
    public Color hoverColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SelectedUnit());
        dstManager.AddComponentData(entity, new HoverTile());
        dstManager.AddComponentData(entity, new UnitSelectionPointer
        {
            value = conversionSystem.GetPrimaryEntity(selectionPointer),
            yPosition = selectionPointerYPosition

        });

        dstManager.AddComponentData(entity, new HoverColor
        {
            value = hoverColor

        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(selectionPointer);
    }
}