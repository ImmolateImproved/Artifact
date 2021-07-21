using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Latios;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections.LowLevel.Unsafe;

public class UnitSelectionViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = sceneBlackboardEntity.GetComponentData<SelectionColors>();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((ref URPMaterialPropertyBaseColor color) =>
            {
                color.Value = (Vector4)unitColors.selectedColor;

            }).Run();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((ref URPMaterialPropertyBaseColor color, in EntityColors defaultColor) =>
            {
                color.Value = (Vector4)defaultColor.defaultColor;

            }).Run();

        CalculateMoveRange();
    }

    private void CalculateMoveRange()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var queue = new NativeQueue<int2>(Allocator.Temp);

        var hashSet = default(NativeHashSet<int2>);

        var neighbors = HexTileNeighbors.Neighbors;

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                hashSet = EntityManager.GetCollectionComponent<MoveRangeSet>(e).moveRangeHashSet;

            }).WithoutBurst().Run();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((in IndexInGrid indexInGrid, in MoveRange moveRange) =>
            {
                hashSet.Clear();

                queue.Enqueue(indexInGrid.value);
                hashSet.Add(indexInGrid.value);

                var currentRange = 0;

                var range = HexTileNeighbors.CalculateTilesCount(moveRange.value, neighbors.Length);

                while (queue.Count > 0)
                {
                    if (currentRange++ >= range)
                        break;

                    var node = queue.Dequeue();

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        var neighborNode = HexTileNeighbors.GetNeightbor(node, neighbors[i]);

                        if (!grid.IndexInRange(neighborNode) || hashSet.Contains(neighborNode))
                            continue;

                        queue.Enqueue(neighborNode);
                        hashSet.Add(neighborNode);
                    }
                }

            }).Run();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                var nodes = hashSet.ToNativeArray(Allocator.Temp);

                if (nodes.Length == 0)
                    return;

                var pathPrefab = sceneBlackboardEntity.GetComponentData<MoveRangePrefab>().prefab;

                var tiles = EntityManager.Instantiate(pathPrefab, nodes.Length, Allocator.Temp);
                for (int i = 0; i < tiles.Length; i++)
                {
                    var node = grid[nodes[i]];
                    var pos = new float3(node.x, 0.4f, node.y);

                    EntityManager.SetComponentData(tiles[i], new Translation { Value = pos });
                }

            }).WithStructuralChanges().Run();
    }
}