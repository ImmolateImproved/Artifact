using Latios;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class BuildGridSystem : SubSystem
{
    EntityQuery m_query;

    public override bool ShouldUpdateSystem()
    {
        return !m_query.IsEmptyIgnoreFilter;
    }

    protected override void OnUpdate()
    {
        GridConfiguration config = default;

        Entities.WithStoreEntityQueryInField(ref m_query).ForEach((in GridConfiguration gridConfig) =>
        {
            config = gridConfig;

        }).Run();

        EntityManager.RemoveComponent<GridConfiguration>(m_query);

        var grid = new Grid(config.columns, config.rows, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        var icb = new InstantiateCommandBuffer<Translation, IndexInGrid>(Allocator.TempJob);

        new InitializeTiles
        {
            config = config,
            grid = grid,
            icb = icb.AsParallelWriter()

        }.ScheduleParallel(config.columns * config.rows, 32, default).Complete();

        SpawnTileText(config, grid);

        icb.Playback(EntityManager);
        Dependency = icb.Dispose(default);

        Entities.WithAll<TileTag>()
            .ForEach((Entity entity, in IndexInGrid gridIndex) =>
            {
                grid.InitTile(gridIndex.value, entity);

            }).Schedule();

        Entities.WithNone<TileTag>()
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

        public InstantiateCommandBuffer<Translation, IndexInGrid>.ParallelWriter icb;

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

            float xPos = (x * xOffset) - (config.columns / 2);
            float yPos = (y * zOffset) - (config.rows / 2);

            if (y % 2 == 1)
            {
                xPos += xOffset / 2;
            }

            var translation = new Translation
            {
                Value = new float3(xPos, 0f, yPos)
            };

            icb.Add(config.tilePrefab, translation, gridPosition, index);
            grid[gridPosition.value] = new float2(xPos, yPos);
        }
    }
}