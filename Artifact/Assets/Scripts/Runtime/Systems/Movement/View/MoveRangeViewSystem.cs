using Unity.Entities;
using Latios;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Rendering;

public partial class MoveRangeViewSystem : SubSystem
{
    private EntityQuery moveRangeTileQuery;

    protected override void OnCreate()
    {
        moveRangeTileQuery = GetEntityQuery(typeof(MoveRangeTileTag));
    }

    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

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

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((Entity e, in IndexInGrid indexInGrid) =>
            {
                var moveRangeSet = sceneBlackboardEntity.GetCollectionComponent<MoveRangeSet>().moveRangeHashSet;

                moveRangeSet.Remove(indexInGrid.value);//remove the player's tile so that it is not displayed

                var nodes = moveRangeSet.ToNativeArray(Allocator.Temp);//copy the nodes into an array without a player node

                moveRangeSet.Add(indexInGrid.value);//return player node back to set

                if (nodes.Length == 0)
                    return;

                var pathPrefab = sceneBlackboardEntity.GetComponentData<MoveRangePrefab>().prefab;

                var tiles = EntityManager.Instantiate(pathPrefab, nodes.Length, Allocator.Temp);
                for (int i = 0; i < tiles.Length; i++)
                {
                    var node = grid[nodes[i]].Value;
                    var pos = new float3(node.x, 0.05f, node.y);

                    EntityManager.SetComponentData(tiles[i], new Translation { Value = pos });
                }

            }).WithStructuralChanges().Run();
    }
}
