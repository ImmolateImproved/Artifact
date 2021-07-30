using Latios;
using Unity.Entities;
using Unity.Mathematics;

public class AttackTargetSelectionSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);
        var mousePosition = GetSingleton<MousePosition>().value;
        var neighbors = HexTileNeighbors.Neighbors;

        if (!TryGetSingletonEntity<Hover>(out var hoverTile))
            return;

        Entities.ForEach((ref AttackNodeData attackTile) =>
            {
                var minDistance = float.MaxValue;
                var closetNode = new int2();

                var currentTile = GetComponent<IndexInGrid>(hoverTile).value;

                for (int i = 0; i < neighbors.Length; i++)
                {
                    var neighborNode = HexTileNeighbors.GetNeightbor(currentTile, neighbors[i]);
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

                attackTile.index = closetNode;

            }).Run();
    }
}