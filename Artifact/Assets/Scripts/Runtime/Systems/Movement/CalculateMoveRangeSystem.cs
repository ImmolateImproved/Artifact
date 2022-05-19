using Unity.Entities;
using Latios;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class CalculateMoveRangeSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        //Add CalculateMoveRange request when the unit was selected
        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                EntityManager.AddComponent<CalculateMoveRange>(e);
                ecb.RemoveComponent<CalculateMoveRange>(e);

            }).WithStructuralChanges().Run();

        //Add CalculateMoveRange request when the unit was stopped
        Entities.WithAll<Movinginternal>().WithNone<Moving>()
             .ForEach((Entity e) =>
             {
                 EntityManager.AddComponent<CalculateMoveRange>(e);
                 ecb.RemoveComponent<CalculateMoveRange>(e);

             }).WithStructuralChanges().Run();

        CalculateMoveRange();
    }

    private void CalculateMoveRange()
    {
        if (!HasSingleton<CalculateMoveRange>())
            return;

        var moveRangeSet = sceneBlackboardEntity.GetCollectionComponent<MoveRangeSet>().moveRangeHashSet;

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var neighbors = HexTileNeighbors.Neighbors;

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((in IndexInGrid indexInGrid, in MoveRange moveRange) =>
            {
                moveRangeSet.Clear();

                var queue = new NativeQueue<int2>(Allocator.Temp);

                queue.Enqueue(indexInGrid.value);
                moveRangeSet.Add(indexInGrid.value);

                var tilesInMoveRange = HexTileNeighbors.CalculateTilesCount(moveRange.value);

                var visitedTiles = new NativeHashSet<int2>(tilesInMoveRange, Allocator.Temp);
                visitedTiles.Add(indexInGrid.value);

                while (visitedTiles.Count() < tilesInMoveRange)
                {
                    var node = queue.Dequeue();

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        var neighborNode = HexTileNeighbors.GetNeighbor(node, neighbors[i]);

                        if (visitedTiles.Add(neighborNode))
                        {
                            queue.Enqueue(neighborNode);
                        }

                        if (grid.HasTile(neighborNode) && !grid.HasUnit(neighborNode))
                        {
                            moveRangeSet.Add(neighborNode);
                        }
                    }
                }

            }).Run();
    }
}