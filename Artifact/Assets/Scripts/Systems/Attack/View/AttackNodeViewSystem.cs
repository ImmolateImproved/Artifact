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

            var attackNode = attackNodeData.node;

            if (!attackNodeView.attackNode.Equals(attackNode))
            {
                attackNodeView.attackNode = attackNode;
                EntityManager.DestroyEntity(attackNodeView.attackPointerEntity);

                attackNodeView.attackPointerEntity = EntityManager.Instantiate(attackNodeView.attackPointerPrefab);

                var hoverTile = GetSingletonEntity<Hover>();
                var hoverNodeIndex = EntityManager.GetComponentData<IndexInGrid>(hoverTile).value;

                var hoverTilePos = grid[hoverNodeIndex];
                var attackTilePos = grid[attackNodeView.attackNode];

                //rotation
                var direction2D = hoverTilePos - attackTilePos;
                var direction = new float3(direction2D.x, 0, direction2D.y);
                var attackTileRotation = quaternion.LookRotationSafe(direction, new float3(0, 1, 0));

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Rotation { Value = attackTileRotation });

                //position
                var selectedUnit = sceneBlackboardEntity.GetComponentData<SelectedUnit>().value;
                var selectedUnitGridPos = EntityManager.GetComponentData<IndexInGrid>(selectedUnit).value;

                var position = new float3(hoverTilePos.x, 1, hoverTilePos.y) + new float3(attackTilePos.x, 1, attackTilePos.y);
                position /= 2;
                position.y = 1;

                EntityManager.SetComponentData(attackNodeView.attackPointerEntity, new Translation { Value = position });
            }

        }).WithStructuralChanges().Run();
    }
}