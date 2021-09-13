using System;
using System.Collections.Generic;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public int moveRange;

    public GameObject selectionPointer;

    public UnitCombatBehaviour combatBehaviour;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveRange { value = moveRange });

        dstManager.AddComponentObject(entity, new UnitCombat
        {
            combatBehaviour = combatBehaviour

        });

        dstManager.AddComponent<UnitTag>(entity);
        dstManager.AddComponent<Selectable>(entity);
        dstManager.AddComponent<AttackState>(entity);
        dstManager.AddComponent<AttackTarget>(entity);

        dstManager.AddComponentData(entity, new UnitSelectionPointer { value = conversionSystem.GetPrimaryEntity(selectionPointer) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(selectionPointer);
    }
}

public class UnitGridPositionConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        //Entities.ForEach((GridAuthoring gridAuthoring) =>
        //{
        //    Entities.ForEach((Transform transform, UnitAuthoring unitAuthoring) =>
        //    {
        //        var ray = new Ray(transform.position, -transform.up);

        //        if (Physics.Raycast(ray, out var hit, 10, LayerMask.NameToLayer("Tile")))
        //        {
        //            var tile = hit.transform.gameObject.GetComponent<TileAuthoring>();
        //            if (tile)
        //            {
        //                var indexInGrid = tile.indexInGrid;

        //                var entity = GetPrimaryEntity(transform);

        //                DstEntityManager.AddComponentData(entity, new IndexInGrid
        //                {
        //                    value = indexInGrid
        //                });

        //                DstEntityManager.AddComponentData(entity, new PreviousGridIndex
        //                {
        //                    value = indexInGrid
        //                });
        //            }
        //        }
        //    });
        //});
    }
}