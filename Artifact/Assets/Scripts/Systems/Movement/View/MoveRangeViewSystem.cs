using Unity.Entities;
using Latios;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;

public class MoveRangeViewSystem : SubSystem
{
    private EntityQuery moveRangeTileQuery;
    private EntityQuery calculateMoveRangeQuery;

    protected override void OnCreate()
    {
        moveRangeTileQuery = GetEntityQuery(typeof(MoveRangeTile));
    }

    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithChangeFilter<Moving>()
             .ForEach((Entity e) =>
             {
                 EntityManager.DestroyEntity(moveRangeTileQuery);

             }).WithStructuralChanges().Run();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((Entity e) =>
            {
                EntityManager.DestroyEntity(moveRangeTileQuery);

            }).WithStructuralChanges().Run();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                EntityManager.AddComponent<CalculateMoveRange>(e);
                ecb.RemoveComponent<CalculateMoveRange>(e);

            }).WithStructuralChanges().Run();

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
        var set = new NativeHashSet<int2>(moveRangeSet.Capacity, Allocator.Temp);

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((in IndexInGrid indexInGrid, in MoveRange moveRange) =>
            {
                moveRangeSet.Clear();

                queue.Enqueue(indexInGrid.value);
                set.Add(indexInGrid.value);

                var currentRange = 0;

                var maxRange = HexTileNeighbors.CalculateTilesCount(moveRange.value, neighbors.Length);

                while (queue.Count > 0)
                {
                    if (currentRange++ >= maxRange)
                        break;

                    var node = queue.Dequeue();

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        var neighborNode = HexTileNeighbors.GetNeightbor(node, neighbors[i]);

                        if (set.Contains(neighborNode))
                            continue;

                        queue.Enqueue(neighborNode);
                        set.Add(neighborNode);

                        if (grid.IndexInRange(neighborNode))
                        {
                            moveRangeSet.Add(neighborNode);
                        }
                    }
                }

            }).Run();

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((Entity e, in IndexInGrid indexInGrid) =>
            {
                var nodes = moveRangeSet.ToNativeArray(Allocator.Temp);

                if (nodes.Length == 0)
                    return;

                //for (int i = 0; i < neighbors.Length; i++)
                //{
                //    var neighborNode = HexTileNeighbors.GetNeightbor(indexInGrid.value, neighbors[i]);
                //    EntityManager.SetComponentData(grid.GetTile(neighborNode), new URPMaterialPropertyBaseColor { Value = (Vector4)Color.gray }); 
                //}

                var pathPrefab = sceneBlackboardEntity.GetComponentData<MoveRangePrefab>().prefab;

                var tiles = EntityManager.Instantiate(pathPrefab, nodes.Length, Allocator.Temp);
                for (int i = 0; i < tiles.Length; i++)
                {
                    var node = grid[nodes[i]];
                    var pos = new float3(node.x, 0.1f, node.y);

                    EntityManager.SetComponentData(tiles[i], new Translation { Value = pos });
                }

            }).WithStructuralChanges().Run();
    }
}