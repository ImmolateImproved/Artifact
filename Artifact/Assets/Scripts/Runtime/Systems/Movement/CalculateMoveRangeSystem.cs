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

        var dijkstraPathfinding = new DijkstraPathfinding(grid);

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((in IndexInGrid indexInGrid, in MoveRange moveRange) =>
            {
                moveRangeSet.Clear();

                dijkstraPathfinding.FindPath(indexInGrid.value, moveRange.value, moveRangeSet);

            }).Run();
    }

    public struct DijkstraPathfinding
    {
        private readonly Grid grid;
        private readonly NativeArray<int2> neighbors;

        public DijkstraPathfinding(Grid grid)
        {
            this.grid = grid;

            neighbors = HexTileNeighbors.Neighbors;
        }

        public void FindPath(int2 start, int moveRange, NativeHashSet<int2> moveRangeSet)
        {
            var costSoFar = new NativeHashMap<int2, int>(64, Allocator.Temp);
            var queue = new NativeQueue<int2>(Allocator.Temp);

            costSoFar[start] = 0;

            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();

                for (int i = 0; i < neighbors.Length; i++)
                {
                    var neighborNode = HexTileNeighbors.GetNeighbor(currentNode, neighbors[i]);

                    var nodeIsValid = grid.IsWalkable(neighborNode) && !moveRangeSet.Contains(neighborNode);

                    if (!nodeIsValid)
                        continue;

                    var newCost = costSoFar[currentNode] + 1;
                    costSoFar.TryGetValue(neighborNode, out var oldCost);

                    if ((oldCost > 0 && newCost >= oldCost) || newCost > moveRange)
                        continue;

                    costSoFar[neighborNode] = newCost;
                    moveRangeSet.Add(neighborNode);
                    queue.Enqueue(neighborNode);
                }
            }

            costSoFar.Dispose();
            queue.Dispose();
        }
    }
}