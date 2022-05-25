using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class TargetSelectionSystem : SubSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<Selected>();
    }

    protected override void OnUpdate()
    {
        if (!TryGetSingletonEntity<Hover>(out var hoverTile))
        {
            return;
        }

        var selectedUnit = sceneBlackboardEntity.GetComponentData<SelectedUnit>().value;

        if (selectedUnit == Entity.Null)
        {
            return;
        }

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);
        var mousePosition = GetSingleton<MousePosition>().value;
        var neighbors = grid.neighbors;

        var moveRangeSet = sceneBlackboardEntity.GetCollectionComponent<MoveRangeSet>().moveRangeHashSet;

        Entities.ForEach((ref TargetManager targetManager) =>
        {
            var currentNode = GetComponent<IndexInGrid>(hoverTile).value;
            var selectedUnitNode = GetComponent<IndexInGrid>(selectedUnit).value;

            targetManager.moveTarget = null;
            targetManager.attackTarget = null;

            var targetIsNotValid = grid.CompareObjects(currentNode, selectedUnit);

            if (targetIsNotValid)
                return;

            var minDistance = float.MaxValue;
            var closestNode = new int2();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeighbor(currentNode, neighbors[i]);

                if (!grid.HasNode(neighborNode))
                    continue;

                var tilePos = grid[neighborNode].Value;

                var distance = math.distancesq(mousePosition, tilePos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = neighborNode;
                }
            }

            int2? moveTarget = grid.HasGridOject(currentNode) ? closestNode : currentNode;
            int2? attackTarget = grid.HasGridOject(currentNode) ? currentNode : closestNode;

            moveTarget = moveRangeSet.Contains(moveTarget.Value) ? moveTarget : null;
            moveTarget = moveTarget == null || (!moveRangeSet.Contains(moveTarget.Value) && neighbors.IsNeightbors(selectedUnitNode, currentNode)) ? selectedUnitNode : moveTarget;

            attackTarget = grid.HasGridOject(attackTarget.Value) ? attackTarget : null;
            attackTarget = (attackTarget != null && !attackTarget.Value.Equals(selectedUnitNode)) ? attackTarget : null;

            targetManager.moveTarget = moveTarget;
            targetManager.attackTarget = attackTarget;

        }).Run();
    }
}