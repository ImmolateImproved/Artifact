using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class InitializeGridSystem : SubSystem
{
    protected override void OnUpdate()
    {
        if (!TryGetSingleton<GridConfig>(out var gridConfig))
        {
            return;
        }

        #region Collection Components Initialization

        //Grid Collection Component
        var grid = new Grid(gridConfig, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        //MoveRangeSet Collection Component
        var moveRangeSet = new MoveRangeSet(6, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(moveRangeSet);

        #endregion

        //Initialize tile positions
        Entities.WithAll<TileTag>()
             .ForEach((in Translation translation, in IndexInGrid indexInGrid) =>
             {
                 grid[indexInGrid.value] = new float2(translation.Value.x, translation.Value.z);

             }).Run();

        //Initialize "Index to tile-entity" HashMap
        Entities.WithAll<TileTag>()
            .ForEach((Entity entity, in IndexInGrid gridIndex) =>
            {
                grid.InitTile(gridIndex.value, entity);

            }).Run();

        Enabled = false;
    }
}