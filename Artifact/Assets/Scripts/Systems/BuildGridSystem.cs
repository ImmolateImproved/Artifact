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

        var grid = new Grid(config.cellsPerRow, config.rows, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        var icb = new InstantiateCommandBuffer<Translation, IndexInGrid>(Allocator.TempJob);

        new InitializeTiles
        {
            gridConfig = config,
            grid = grid,
            icb = icb.AsParallelWriter()

        }.ScheduleParallel(config.cellsPerRow * config.rows, 32, default).Complete();

        var tileText = this.GetSingleton<TileText>();
        if (tileText.showTileIndices)
        {
            var parent = new GameObject("Text").transform;

            for (int index = 0; index < config.cellsPerRow * config.rows; index++)
            {
                int x = index % config.cellsPerRow;
                int y = index / config.cellsPerRow;

                var pos = grid[new int2(x, y)];
                var text = GameObject.Instantiate(tileText.textPrefab, new float3(pos.x, 0.3f, pos.y), Quaternion.Euler(90, 0, 0), parent);
                text.text = $"{x} {y}";
            }
        }

        icb.Playback(EntityManager);
        Dependency = icb.Dispose(default);

        Entities.WithNone<TileTag>()
            .ForEach((Entity entity, ref Translation translation, in IndexInGrid gridIndex) =>
            {
                grid.SetUnit(gridIndex.value, entity);

                var position = grid[gridIndex.value];
                translation.Value = new float3(position.x, translation.Value.y, position.y);

            }).Schedule();
    }

    [BurstCompile]
    struct InitializeTiles : IJobFor
    {
        public GridConfiguration gridConfig;

        public Grid grid;

        public InstantiateCommandBuffer<Translation, IndexInGrid>.ParallelWriter icb;

        private const float xOffset = 1.05f;
        private const float zOffset = 0.9f;

        public void Execute(int index)
        {
            int x = index % gridConfig.cellsPerRow;
            int y = index / gridConfig.cellsPerRow;

            var gridPosition = new IndexInGrid
            {
                value = new int2(x, y)
            };

            float xPos = x * xOffset;
            float yPos = y * zOffset;

            if (y % 2 == 1)
            {
                xPos += xOffset / 2;
            }
            var translation = new Translation
            {
                Value = new float3(xPos, 0f, yPos)

            };

            icb.Add(gridConfig.tilePrefab, translation, gridPosition, index);
            grid[new int2(x, y)] = new float2(xPos, yPos);
        }
    }
}