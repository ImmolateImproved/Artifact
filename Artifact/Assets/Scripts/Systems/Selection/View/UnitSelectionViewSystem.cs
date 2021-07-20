using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Latios;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

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

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var neighbors = new NativeArray<int2>(6, Allocator.Temp)
        {
            [0] = new int2(-1, 0),
            [1] = new int2(-1, 1),
            [2] = new int2(0, 1),
            [3] = new int2(1, 0),
            [4] = new int2(0, -1),
            [5] = new int2(-1, -1)
        };

        var pathPrefab = sceneBlackboardEntity.GetComponentData<PathPrefab>().prefab;

        var nodes = new NativeList<int2>(15, Allocator.Temp);

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
                    .ForEach((in IndexInGrid indexInGrid) =>
                    {
                        for (int i = 0; i < neighbors.Length; i++)
                        {
                            for (int j = 1; j < 8; j++)
                            {
                                var neighborNode = GetNeightbor(indexInGrid.value, neighbors[i] * j);
                                
                                if (!grid.IndexInRange(neighborNode))
                                    continue;

                                nodes.Add(neighborNode);
                            }
                        }

                    }).Run();

        if (nodes.Length == 0)
            return;

        var tiles = EntityManager.Instantiate(pathPrefab, nodes.Length, Allocator.Temp);
        for (int i = 0; i < tiles.Length; i++)
        {
            var node = grid[nodes[i]];
            var pos = new float3(node.x, 0.4f, node.y);

            EntityManager.SetComponentData(tiles[i], new Translation { Value = pos });
        }
    }

    private static int2 GetNeightbor(int2 currentNode, int2 neighborOffset)
    {
        //for hex grid, if we on odd row - change the sign of a neighborOffset
        neighborOffset = math.select(neighborOffset, -neighborOffset, currentNode.y % 2 == 1);

        return currentNode + neighborOffset;
    }
}