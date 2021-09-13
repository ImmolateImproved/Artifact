using Unity.Entities;
using Latios;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;

public class MoveRangeViewSystem : SubSystem
{
    private EntityQuery moveRangeTileQuery;

    protected override void OnCreate()
    {
        moveRangeTileQuery = GetEntityQuery(typeof(MoveRangeTile));
    }

    protected override void OnUpdate()
    {
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

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.WithAll<CalculateMoveRange>()
            .ForEach((Entity e, in IndexInGrid indexInGrid) =>
            {
                var moveRangeSet = EntityManager.GetCollectionComponent<MoveRangeSet>(e).moveRangeHashSet;

                var nodes = moveRangeSet.ToNativeArray(Allocator.Temp);

                if (nodes.Length == 0)
                    return;

                var pathPrefab = sceneBlackboardEntity.GetComponentData<MoveRangePrefab>().prefab;

                var tiles = EntityManager.Instantiate(pathPrefab, nodes.Length, Allocator.Temp);
                for (int i = 0; i < tiles.Length; i++)
                {
                    var node = grid[nodes[i]];
                    var pos = new float3(node.x, 0.05f, node.y);

                    EntityManager.SetComponentData(tiles[i], new Translation { Value = pos });
                }

            }).WithStructuralChanges().Run();
    }
}
