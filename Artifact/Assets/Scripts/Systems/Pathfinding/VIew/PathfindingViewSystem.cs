using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class PathfindingViewSystem : SubSystem
{
    private EntityQuery pathTileQuery;

    protected override void OnCreate()
    {
        pathTileQuery = GetEntityQuery(typeof(PathTile));
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<ActionRequest, DrawPath>()
            .ForEach((ref DynamicBuffer<UnitPath> path) =>
            {
                var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);
                var pathPrefab = sceneBlackboardEntity.GetComponentData<PathPrefab>().prefab;

                var pathArray = path.Reinterpret<int2>().ToNativeArray(Allocator.Temp);

                EntityManager.DestroyEntity(pathTileQuery);
                var tiles = EntityManager.Instantiate(pathPrefab, pathArray.Length, Allocator.Temp);

                for (int i = 0; i < tiles.Length; i++)
                {
                    var pos = grid[pathArray[i]];

                    EntityManager.SetComponentData(tiles[i], new Translation
                    {
                        Value = new float3(pos.x, 0.1f, pos.y)
                    });
                }

            }).WithStructuralChanges().Run();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((Entity e) =>
            {
                EntityManager.DestroyEntity(pathTileQuery);

            }).WithStructuralChanges().Run();

    }
}