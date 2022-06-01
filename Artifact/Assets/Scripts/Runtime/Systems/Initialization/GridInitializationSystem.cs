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
    public GridSpawner gridSpawner;

    public EntityManager entityManager;

    public void Execute()
    {
        var nodes = BuildGridBFSAxial();

        var spawnECB = new InstantiateCommandBuffer<Translation, NonUniformScale, IndexInGrid>(Allocator.TempJob);

        foreach (var node in nodes)
        {
            var position = GridConfig.NodeToPosition(node, config.TileSlotRadius);
            position.y = -1;

            var tileSize = config.TileSize;

            var translation = new Translation { Value = position };
            var scale = new NonUniformScale { Value = new Vector3(tileSize, 1, tileSize) };
            var gridIndex = new IndexInGrid { current = node };

            spawnECB.Add(gridSpawner.prefab, translation, scale, gridIndex);
        }

        nodes.Dispose();

        spawnECB.Playback(entityManager);
        spawnECB.Dispose();
    }

    private NativeHashSet<int2> BuildGridBFSAxial()
    {
        var neighbors = HexTileNeighbors.Neighbors;

        var tileInGridRadius = HexTileNeighbors.CalculateTilesCount(config.GridRadius);

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

[AlwaysUpdateSystem]
public partial class GridInitializationSystem : SubSystem
{
    public override bool ShouldUpdateSystem()
    {
        var shouldUpdate = sceneBlackboardEntity.HasComponent<GridConfig>() && !sceneBlackboardEntity.HasComponent<GridInitializedTag>();

        return shouldUpdate;
    }

    protected override void OnUpdate()
    {
        var gridConfig = sceneBlackboardEntity.GetComponentData<GridConfig>();
        var gridSpawner = sceneBlackboardEntity.GetComponentData<GridSpawner>();

        var spawnGridJob = new GridSpawnerJob
        {
            config = gridConfig,
            gridSpawner = gridSpawner,
            entityManager = EntityManager
        };

        spawnGridJob.Execute();

        #region Collection Components Initialization

        //Grid Collection Component

        var grid = new Grid(gridConfig.NodesCount);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        var tileGridData = new TileGridData(gridConfig.NodesCount);
        sceneBlackboardEntity.AddCollectionComponent(tileGridData);
        #endregion

        var initialized = false;

        //Initialize tile positions and "Index to tile-entity" HashMap
        Entities.WithAll<TileTag>()
             .ForEach((Entity entity, in Translation translation, in IndexInGrid indexInGrid) =>
             {
                 var nodePosition = new float2(translation.Value.x, translation.Value.z);

                 grid.SetNodePosition(indexInGrid.current, nodePosition);
                 tileGridData.InitTile(indexInGrid.current, entity);

                 initialized = true;

             }).Run();

        if (initialized)
        {
            sceneBlackboardEntity.AddComponent<GridInitializedTag>();
        }

    }
}