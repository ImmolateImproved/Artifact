using Latios;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AttackTargetSelectionViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref AttackNodeView attackNodeView, in AttackTargetManager attackTargetData) =>
        {
            if (attackTargetData.attackNode.Equals(-1))
            {
                EntityManager.DestroyEntity(attackNodeView.attackPointerEntity);
                attackNodeView.attackNode = -1;
                return;
            }

            var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

            var attackNode = attackTargetData.attackNode;
            var attackTarget = attackTargetData.attackTarget;

            if (!attackNodeView.attackNode.Equals(attackNode) || !attackNodeView.attackTarget.Equals(attackTarget))
            {
                attackNodeView.attackNode = attackNode;
                attackNodeView.attackTarget = attackTarget;
                EntityManager.DestroyEntity(attackNodeView.attackPointerEntity);

                attackNodeView.attackPointerEntity = EntityManager.Instantiate(attackNodeView.attackPointerPrefab);

                var attackNodePos = grid[attackNode];
                var attackTargetPos = grid[attackTarget];

                //rotation
                var direction2D = attackTargetPos - attackNodePos;
                var direction = new float3(direction2D.x, 0, direction2D.y);
                var attackTileRotation = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Rotation { Value = attackTileRotation });

                //position
                var selectedUnit = sceneBlackboardEntity.GetComponentData<SelectedUnit>().value;
                var selectedUnitGridPos = EntityManager.GetComponentData<IndexInGrid>(selectedUnit).value;

                var position = new float3(attackTargetPos.x, 1, attackTargetPos.y) + new float3(attackNodePos.x, 1, attackNodePos.y);
                position /= 2;
                position.y = 1;

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Translation { Value = position });
            }

        }).WithStructuralChanges().Run();
    }
}