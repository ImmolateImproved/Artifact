using Unity.Entities;
using Latios;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public partial class CalculateMoveRangeSystem : SubSystem
{
    private EntityQuery calculateMoveRangeQuery;

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
        if (calculateMoveRangeQuery.IsEmpty)
            return;

        var moveRangeSet = default(NativeHashSet<int2>);

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((Entity e) =>
            {
                moveRangeSet = EntityManager.GetCollectionComponent<MoveRangeSet>(e).moveRangeHashSet;

            }).WithStoreEntityQueryInField(ref calculateMoveRangeQuery).WithoutBurst().Run();

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var neighbors = HexTileNeighbors.Neighbors;
        var queue = new NativeQueue<int2>(Allocator.Temp);

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((in IndexInGrid indexInGrid, in MoveRange moveRange) =>
            {
                moveRangeSet.Clear();

                queue.Enqueue(indexInGrid.value);
                moveRangeSet.Add(indexInGrid.value);

                var maxRange = HexTileNeighbors.CalculateTilesCount(moveRange.value);

                var visitedTiles = new NativeHashSet<int2>(maxRange, Allocator.Temp);

                while (queue.Count > 0)
                {
                    if (visitedTiles.Count() > maxRange)
                        break;

                    var node = queue.Dequeue();

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        var neighborNode = HexTileNeighbors.GetNeighbor(node, neighbors[i]);

                        if (visitedTiles.Add(neighborNode))
                        {
                            queue.Enqueue(neighborNode);
                        }

                        if (grid.IndexInRange(neighborNode) && !grid.HasUnit(neighborNode))
                        {
                            moveRangeSet.Add(neighborNode);
                        }
                    }
                }

            }).Run();
    }
}