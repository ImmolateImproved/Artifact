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

public class BuildGridSystem : SubSystem
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
        GridConfiguration config = default;

        Entities.WithStoreEntityQueryInField(ref gridConfigQuery).ForEach((in GridConfiguration gridConfig) =>
        {
            config = gridConfig;

        }).Run();

        EntityManager.RemoveComponent<GridConfiguration>(gridConfigQuery);

        var grid = new Grid(config.columns, config.rows, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        //var icb = new InstantiateCommandBuffer<Translation, IndexInGrid, Scale>(Allocator.TempJob);

        //new InitializeTiles
        //{
        //    config = config,
        //    grid = grid,
        //    icb = icb.AsParallelWriter()

        //}.ScheduleParallel(config.columns * config.rows, 32, default).Complete();

        //icb.Playback(EntityManager);
        //Dependency = icb.Dispose(default);

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

        Entities.WithAll<TileTag>()
             .ForEach((in Translation translation, in IndexInGrid indexInGrid) =>
             {
                 grid[indexInGrid.value] = new float2(translation.Value.x, translation.Value.z);

             }).Run();

        SpawnTileText(config, grid);

        Entities.WithAll<TileTag>()
            .ForEach((Entity entity, in IndexInGrid gridIndex) =>
            {
                grid.InitTile(gridIndex.value, entity);

            }).ScheduleParallel();

        Entities.WithAll<UnitTag>()
            .ForEach((Entity entity, ref Translation translation, in IndexInGrid gridIndex) =>
            {
                grid.SetUnit(gridIndex.value, entity);

                var position = grid[gridIndex.value];
                translation.Value = new float3(position.x, translation.Value.y, position.y);

            }).Schedule();
    }

    private void SpawnTileText(GridConfiguration config, Grid grid)
    {
        var tileText = this.GetSingleton<TileText>();
        if (tileText.showTileIndices)
        {
            var parent = new GameObject("Text").transform;

            for (int index = 0; index < config.columns * config.rows; index++)
            {
                int x = index % config.columns;
                int y = index / config.columns;

                var pos = grid[new int2(x, y)];
                var text = GameObject.Instantiate(tileText.textPrefab, new float3(pos.x, 0.201f, pos.y), Quaternion.Euler(90, 0, 0), parent);
                text.text = $"{x}.{y}";
            }
        }
    }

    [BurstCompile]
    struct InitializeTiles : IJobFor
    {
        public GridConfiguration config;

        public Grid grid;

        public InstantiateCommandBuffer<Translation, IndexInGrid, Scale>.ParallelWriter icb;

        private const float xOffset = 1.05f;
        private const float zOffset = 0.9f;

        public void Execute(int index)
        {
            int x = index % config.columns;
            int y = index / config.columns;

            var gridPosition = new IndexInGrid
            {
                value = new int2(x, y)
            };

            float xPos = (x * xOffset * config.tileScale) - (config.columns / 2);
            float yPos = (y * zOffset * config.tileScale) - (config.rows / 2);

            if (y % 2 == 1)
            {
                xPos += xOffset * config.tileScale / 2;
            }

            var translation = new Translation
            {
                Value = new float3(xPos, 0f, yPos)
            };

            icb.Add(config.tilePrefab, translation, gridPosition, new Scale { Value = config.tileScale }, index);
            grid[gridPosition.value] = new float2(xPos, yPos);
        }
    }
}