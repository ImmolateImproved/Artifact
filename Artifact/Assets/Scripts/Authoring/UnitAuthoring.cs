using System;
using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int moveRange;

    public UnitCombatBehaviour combatBehaviour;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new MoveRange { value = moveRange });

        dstManager.AddComponentObject(entity, new UnitCombat
        {
            combatBehaviour = combatBehaviour

        });

        dstManager.AddComponent<AttackState>(entity);
        dstManager.AddComponent<Selectable>(entity);
        dstManager.AddComponent<AttackTarget>(entity);
    }
}

public class UnitGridPositionConversionSystem : GameObjectConversionSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((GridAuthoring gridAuthoring) =>
        {
            Entities.ForEach((Transform transform, UnitAuthoring unitAuthoring) =>
            {
                var xPos = (int)((transform.position.x) + (gridAuthoring.width) / 2);
                var yPos = (int)((transform.position.z) + (gridAuthoring.height) / 2);

                var indexInGrid = new int2(xPos, yPos);

                var entity = GetPrimaryEntity(transform);

                DstEntityManager.AddComponentData(entity, new IndexInGrid
                {
                    value = indexInGrid
                });

                DstEntityManager.AddComponentData(entity, new PreviousGridIndex
                {
                    value = indexInGrid
                });

            });
        });
    }
}