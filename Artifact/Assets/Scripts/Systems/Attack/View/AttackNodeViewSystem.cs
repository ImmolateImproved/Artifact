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
                EntityManager.DestroyEntity(attackNodeView.attackTileEntity);
                attackNodeView.attackNode = -1;
                return;
            }

            var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

            var newAttackNode = attackNodeData.node;

            if (!attackNodeView.attackNode.Equals(newAttackNode))
            {
                attackNodeView.attackNode = newAttackNode;
                EntityManager.DestroyEntity(attackNodeView.attackTileEntity);

                attackNodeView.attackTileEntity = EntityManager.Instantiate(attackNodeView.attackTilePrefab);

                var pos = grid[attackNodeView.attackNode];
                EntityManager.SetComponentData(attackNodeView.attackTileEntity, new Translation { Value = new float3(pos.x, 0.3f, pos.y) });
            }

        }).WithStructuralChanges().Run();
    }
}