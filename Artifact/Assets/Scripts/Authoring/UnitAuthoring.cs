using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public bool selectable;
    public GameObject selectionPointer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<UnitTag>(entity);

        dstManager.AddComponent<IndexInGrid>(entity);
        dstManager.AddComponent<PreviousGridIndex>(entity);

        if (selectable)
        {
            dstManager.AddComponent<Selectable>(entity);
            dstManager.AddComponentData(entity, new UnitSelectionPointer { value = conversionSystem.GetPrimaryEntity(selectionPointer) });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(selectionPointer);
    }
}