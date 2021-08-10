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

        Entities.ForEach((ref AttackNodeData attackNode) =>
        {
            var hoverNode = GetComponent<IndexInGrid>(hoverTile).value;

            attackNode.index = -1;

            var targetIsInvalid =
            selectedUnit.value == Entity.Null
            || !grid.HasUnit(hoverNode)
            || selectedUnit.value == grid.GetUnit(hoverNode);

            if (targetIsInvalid)
                return;

            var minDistance = float.MaxValue;
            var closetNode = new int2();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeightbor(hoverNode, neighbors[i]);
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

            attackNode.index = moveRangeSet.Contains(closetNode) ? closetNode : -1;

        }).Run();
    }
}