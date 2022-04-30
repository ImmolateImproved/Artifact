using Latios;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;
using UnityEngine;

public partial class InitializeGridSystem : SubSystem
{
    private EntityQuery gridConfigQuery;

    private BuildPhysicsWorld physicsWorld;

    protected override void OnCreate()
    {
        physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    public override bool ShouldUpdateSystem()
    {
        return !gridConfigQuery.IsEmptyIgnoreFilter;
    }

    protected override void OnUpdate()
    {
        var config = default(GridConfiguration);
        //Get config data
        Entities.ForEach((in GridConfiguration gridConfig) =>
        {
            config = gridConfig;

        }).WithStoreEntityQueryInField(ref gridConfigQuery).Run();

        EntityManager.RemoveComponent<GridConfiguration>(gridConfigQuery);

        //Initialize Grid Collection Component
        var grid = new Grid(config, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        //Initialize units grid index
        Entities.WithAll<UnitTag>().WithNone<MoveRangeAssociated>()
            .ForEach((Entity e, Translation translation) =>
            {
                var selectionFilter = GetSingleton<SelectionFilter>();

                var rayInput = new RaycastInput
                {
                    Start = translation.Value,
                    End = translation.Value + new float3(0, -1, 0) * 10,
                    Filter = selectionFilter.collisionFilter
                };

                if (physicsWorld.PhysicsWorld.CollisionWorld.CastRay(rayInput, out var raycastHit))
                {
                    if (EntityManager.HasComponent<IndexInGrid>(raycastHit.Entity))
                    {
                        var tileIndex = EntityManager.GetComponentData<IndexInGrid>(raycastHit.Entity);

                        EntityManager.AddComponentData(e, new IndexInGrid { value = tileIndex.value });
                        EntityManager.AddComponentData(e, new PreviousGridIndex { value = tileIndex.value });
                    }
                }

            }).WithStructuralChanges().Run();

        //Initialize tile positions
        Entities.WithAll<TileTag>()
             .ForEach((in Translation translation, in IndexInGrid indexInGrid) =>
             {
                 grid[indexInGrid.value] = new float2(translation.Value.x, translation.Value.z);

             }).Run();

        SpawnTileText(config, grid);

        //Initialize "Index to tile-entity" HashMap
        Entities.WithAll<TileTag>()
            .ForEach((Entity entity, in IndexInGrid gridIndex) =>
            {
                grid.InitTile(gridIndex.value, entity);

            }).Run();

        //Initialize units position
        Entities.WithAll<UnitTag>()
            .ForEach((Entity entity, ref Translation translation, in IndexInGrid gridIndex) =>
            {
                grid.SetUnit(gridIndex.value, entity);

                var position = grid[gridIndex.value];
                translation.Value = new float3(position.x, translation.Value.y, position.y);

            }).Run();
    }

    private void SpawnTileText(GridConfiguration config, Grid grid)
    {
        var tileText = this.GetSingleton<TileText>();
        if (tileText.showTileIndices)
        {
            var parent = new GameObject("Text").transform;

            for (int index = 0; index < config.width * config.height; index++)
            {
                int x = index % config.width;
                int y = index / config.width;

                var pos = grid[new int2(x, y)];
                var text = GameObject.Instantiate(tileText.textPrefab, new float3(pos.x, 0.201f, pos.y), Quaternion.Euler(90, 0, 0), parent);
                text.text = $"{x}.{y}";
            }
        }
    }
}