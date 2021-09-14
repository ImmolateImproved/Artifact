using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public class AttackTargetSelectionSystem : SubSystem
{
    protected override void OnUpdate()
    {
        if (!TryGetSingletonEntity<Hover>(out var hoverTile))
            return;

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);
        var selectedUnit = sceneBlackboardEntity.GetComponentData<SelectedUnit>();
        var mousePosition = GetSingleton<MousePosition>().value;
        var neighbors = HexTileNeighbors.Neighbors;

        var moveRangeSet = default(NativeHashSet<int2>);

        if (selectedUnit.value != Entity.Null)
        {
            moveRangeSet = EntityManager.GetCollectionComponent<MoveRangeSet>(selectedUnit.value, true).moveRangeHashSet;
        }

        Entities.ForEach((ref AttackTargetManager attackNode) =>
        {
            var currentNode = GetComponent<IndexInGrid>(hoverTile).value;

            attackNode.attackNode = -1;
            attackNode.attackTarget = -1;

            var targetIsValid =
            selectedUnit.value != Entity.Null
            && selectedUnit.value != grid.GetUnit(currentNode);

            if (!targetIsValid)
                return;

            var minDistance = float.MaxValue;
            var closetNode = new int2();

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
                    closetNode = neighborNode;
                }
            }

            if (grid.HasUnit(currentNode))
            {
                var isValid = moveRangeSet.Contains(closetNode);

                attackNode.attackNode = isValid ? closetNode : -1;
                attackNode.attackTarget = currentNode;
            }
            else if (grid.HasUnit(closetNode))
            {
                var isValid = (grid.GetUnit(closetNode) != selectedUnit.value) && moveRangeSet.Contains(currentNode);

                attackNode.attackNode = isValid ? currentNode : -1;
                attackNode.attackTarget = isValid ? closetNode : -1;
            }

        }).Run();
    }
}