using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class TargetSelectionSystem : SubSystem
{
    protected override void OnUpdate()
    {
        if (!TryGetSingletonEntity<Hover>(out var hoverTile))
            return;

        var selectedUnit = sceneBlackboardEntity.GetComponentData<SelectedUnit>();

        if (selectedUnit.value == Entity.Null) return;

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);
        var mousePosition = GetSingleton<MousePosition>().value;
        var neighbors = HexTileNeighbors.Neighbors;

        var moveRangeSet = EntityManager.GetCollectionComponent<MoveRangeSet>(selectedUnit.value, true).moveRangeHashSet;

        Entities.ForEach((ref TargetManager targetManager) =>
        {
            var currentNode = GetComponent<IndexInGrid>(hoverTile).value;
            var selectedUnitNode = GetComponent<IndexInGrid>(selectedUnit.value).value;

            targetManager.moveTarget = -1;
            targetManager.attackTarget = -1;

            var targetIsValid = selectedUnit.value != grid.GetUnit(currentNode);

            if (!targetIsValid)
                return;

            var minDistance = float.MaxValue;
            var closestNode = new int2();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeightbor(currentNode, neighbors[i]);
                if (!grid.IndexInRange(neighborNode))
                    continue;

                var tilePos = grid[neighborNode];

                var distance = math.distancesq(mousePosition, tilePos);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestNode = neighborNode;
                }
            }

            var moveTarget = grid.HasUnit(currentNode) ? closestNode : currentNode;
            var attackTarget = grid.HasUnit(currentNode) ? currentNode : closestNode;

            moveTarget = moveRangeSet.Contains(moveTarget) ? moveTarget : -1;
            moveTarget = (!moveRangeSet.Contains(moveTarget) && neighbors.IsNeightbors(selectedUnitNode, currentNode)) ? selectedUnitNode : moveTarget;

            attackTarget = grid.HasUnit(attackTarget) ? attackTarget : -1;
            attackTarget = (!attackTarget.Equals(selectedUnitNode)) ? attackTarget : -1;

            targetManager.moveTarget = moveTarget;
            targetManager.attackTarget = attackTarget;

        }).Run();
    }
}