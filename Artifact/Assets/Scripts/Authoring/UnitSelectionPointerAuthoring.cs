using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class UnitSelectionPointerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject selectionPointer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new UnitSelectionPointer { value = conversionSystem.GetPrimaryEntity(selectionPointer) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(selectionPointer);
    }
}