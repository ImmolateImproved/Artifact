using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public struct GridSpawnerJob
{
    public GridConfig config;

    public int2[] neighbors;

    public EntityCommandBuffer spawnECB;

    public GridSpawnerJob(GridConfig config, EntityCommandBuffer spawnECB) : this()
    {
        this.config = config;
        this.spawnECB = spawnECB;
        neighbors = HexTileNeighbors.Neighbors;
    }

    public void Execute(GridSpawner gridSpawner)
    {
        GenerateCircleGrid(gridSpawner.prefab);
    }

    private void GenerateCircleGrid(Entity prefab)
    {
        var nodes = BuildGridBFSAxial();

        foreach (var node in nodes)
        {
            var position = GridConfig.NodeToPosition(node, config.tileSlotRadius);
            position.y = -1;

            var tileSize = config.tileSize;

            var tile = spawnECB.Instantiate(prefab);
            spawnECB.SetComponent(tile, new Translation { Value = position });
            spawnECB.AddComponent(tile, new NonUniformScale { Value = new Vector3(tileSize, 1, tileSize) });
            spawnECB.SetComponent(tile, new IndexInGrid { value = node });
        }

        nodes.Dispose();
    }

    private NativeHashSet<int2> BuildGridBFSAxial()
    {
        var tileInGridRadius = HexTileNeighbors.CalculateTilesCount(config.gridRadius);

        var queue = new NativeQueue<int2>(Allocator.Temp);
        var visited = new NativeHashSet<int2>(tileInGridRadius, Allocator.Temp);

        queue.Enqueue(0);
        visited.Add(0);

        while (visited.Count() < tileInGridRadius)
        {
            var node = queue.Dequeue();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeighborNode(node, neighbors[i]);

                if (visited.Add(neighborNode))
                {
                    queue.Enqueue(neighborNode);
                }
            }
        }

        queue.Dispose();

        return visited;
    }
}

public partial class GridInitializationSystem : SubSystem
{
    public override bool ShouldUpdateSystem()
    {
        return !sceneBlackboardEntity.HasComponent<GridInitializedTag>();
    }

    protected override void OnUpdate()
    {
        if (!TryGetSingleton<GridConfig>(out var gridConfig))
        {
            return;
        }

        var tileSpawnECB = new EntityCommandBuffer(Allocator.Temp);

        var spawnGridJob = new GridSpawnerJob(gridConfig, tileSpawnECB);

        spawnGridJob.Execute(GetSingleton<GridSpawner>());

        tileSpawnECB.Playback(EntityManager);
        #region Collection Components Initialization

        //Grid Collection Component
        var grid = new Grid(gridConfig);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        #endregion

        var initialized = false;

        //Initialize tile positions and "Index to tile-entity" HashMap
        Entities.WithAll<TileTag>()
             .ForEach((Entity entity, in Translation translation, in IndexInGrid indexInGrid) =>
             {
                 var nodePosition = new float2(translation.Value.x, translation.Value.z);

                 grid.SetNodePosition(indexInGrid.value, nodePosition);
                 grid.InitTile(indexInGrid.value, entity);

                 initialized = true;

             }).Run();

        if (initialized)
        {
            sceneBlackboardEntity.AddComponent<GridInitializedTag>();
        }

    }
}