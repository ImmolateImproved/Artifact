using Latios;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class AttackNodeViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref AttackNodeView attackNodeView, in AttackNodeManager attackNodeData) =>
        {
            if (attackNodeData.node.Equals(-1))
            {
                EntityManager.DestroyEntity(attackNodeView.attackPointerEntity);
                attackNodeView.attackNode = -1;
                return;
            }

            var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

            var newAttackNode = attackNodeData.node;

            if (!attackNodeView.attackNode.Equals(newAttackNode))
            {
                attackNodeView.attackNode = newAttackNode;
                EntityManager.DestroyEntity(attackNodeView.attackPointerEntity);

                attackNodeView.attackPointerEntity = EntityManager.Instantiate(attackNodeView.attackPointerPrefab);

                //rotation
                var selectedTile = GetSingletonEntity<Hover>();
                var selectedNodeIndex = EntityManager.GetComponentData<IndexInGrid>(selectedTile).value;
                var direction2D = grid[selectedNodeIndex] - grid[attackNodeView.attackNode];
                var attackTileRotation = quaternion.LookRotationSafe(new float3(direction2D.x, 0, direction2D.y), new float3(0, 1, 0));

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Rotation { Value = attackTileRotation });

                //position
                var selectedUnit = sceneBlackboardEntity.GetComponentData<SelectedUnit>().value;
                var selectedUnitGridPos = EntityManager.GetComponentData<IndexInGrid>(selectedUnit).value;
                var attackNodeIsPlayerNode = selectedUnitGridPos.Equals(newAttackNode);

                var position2D = attackNodeIsPlayerNode ? grid[selectedNodeIndex] : grid[attackNodeView.attackNode];

                var position = new float3(position2D.x, 0.5f, position2D.y);
                var direction = new float3(direction2D.x, 0, direction2D.y);

                if (attackNodeIsPlayerNode)
                {
                    position.y = 1.5f;
                    position -= direction/2;
                }

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Translation { Value = position });
            }

        }).WithStructuralChanges().Run();
    }
}