using Unity.Entities;
using Latios;
using UnityEngine;

public class AttackSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.WithAll<DecisionRequest>()
           .ForEach((Entity e, in AttackState attackState) =>
           {
               if (attackState.attack)
               {

               }

           }).WithStructuralChanges().Run();

        Entities.WithAny<DecisionRequest, Movinginternal>().WithNone<Moving>()
           .ForEach((Entity e, UnitCombat unitCombat, in AttackTarget attackTarget, in AttackState attackState) =>
           {
               if (!attackState.attack)
                   return;

               var targetUnit = EntityManager.GetComponentObject<UnitCombat>(grid.GetUnit(attackTarget.node));
               targetUnit.combatBehaviour.TakeDamage(unitCombat.combatBehaviour);

           }).WithoutBurst().Run();
    }
}