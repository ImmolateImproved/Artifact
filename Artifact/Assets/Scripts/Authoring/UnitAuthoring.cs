using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int moveRange;

    public GameObject selectionPointer;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveRange { value = moveRange });

        dstManager.AddComponent<UnitTag>(entity);
        dstManager.AddComponent<MoveDestination>(entity);
        dstManager.AddComponent<Selectable>(entity);
        dstManager.AddComponent<AttackState>(entity);
        dstManager.AddComponent<AttackTarget>(entity);

        dstManager.AddComponent<IndexInGrid>(entity);
        dstManager.AddComponent<PreviousGridIndex>(entity);

        dstManager.AddComponentData(entity, new UnitSelectionPointer { value = conversionSystem.GetPrimaryEntity(selectionPointer) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(selectionPointer);
    }
}