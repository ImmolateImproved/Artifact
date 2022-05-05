using Latios;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class AttackTargetSelectionViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref AttackNodeView attackNodeView, in TargetManager targetData) =>
        {
            if (targetData.moveTarget == null || targetData.attackTarget == null)
            {
                EntityManager.DestroyEntity(attackNodeView.attackPointerEntity);
                return;
            }

            var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

            var moveNode = targetData.moveTarget.Value;
            var attackTarget = targetData.attackTarget.Value;

            if (!attackNodeView.moveNode.Equals(moveNode) || !attackNodeView.attackTarget.Equals(attackTarget))
            {
                attackNodeView.moveNode = moveNode;
                attackNodeView.attackTarget = attackTarget;
                EntityManager.DestroyEntity(attackNodeView.attackPointerEntity);

                attackNodeView.attackPointerEntity = EntityManager.Instantiate(attackNodeView.attackPointerPrefab);

                var moveNodePos = grid[moveNode].Value;
                var attackTargetPos = grid[attackTarget].Value;

                //rotation
                var direction2D = attackTargetPos - moveNodePos;
                var direction = new float3(direction2D.x, 0, direction2D.y);
                var attackTileRotation = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Rotation { Value = attackTileRotation });

                //position
                var selectedUnit = sceneBlackboardEntity.GetComponentData<SelectedUnit>().value;
                var selectedUnitGridPos = EntityManager.GetComponentData<IndexInGrid>(selectedUnit).value;

                var position = new float3(attackTargetPos.x, 1, attackTargetPos.y) + new float3(moveNodePos.x, 1, moveNodePos.y);
                position /= 2;
                position.y = 1;

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Translation { Value = position });
            }

        }).WithStructuralChanges().Run();
    }
}