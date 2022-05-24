using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class InitializeGridSystem : SubSystem
{
    public override bool ShouldUpdateSystem()
    {
        return !sceneBlackboardEntity.HasComponent<GridInitialized>();
    }

    protected override void OnUpdate()
    {
        if (!TryGetSingleton<GridConfig>(out var gridConfig))
        {
            return;
        }

        #region Collection Components Initialization

        //Grid Collection Component
        var grid = new Grid(gridConfig);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        //MoveRangeSet Collection Component
        var moveRangeSet = new MoveRangeSet(6, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(moveRangeSet);

        #endregion

        var initialized = false;

        //Initialize tile positions and "Index to tile-entity" HashMap
        Entities.WithAll<TileTag>()
             .ForEach((Entity entity, in Translation translation, in IndexInGrid indexInGrid) =>
             {
                 grid[indexInGrid.value] = new float2(translation.Value.x, translation.Value.z);
                 grid.InitTile(indexInGrid.value, entity);

                 initialized = true;

             }).Run();

        if (initialized)
        {
            sceneBlackboardEntity.AddComponent<GridInitialized>();
        }

    }
}