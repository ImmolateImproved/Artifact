using Latios;
using Unity.Entities;
using UnityEngine;

public partial class UpdateGridSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.WithAll<Moving>()
            .ForEach((Entity e, in IndexInGrid indexInGrid, in PreviousGridIndex previousGridIndex, in InDistance inDistance) =>
            {
                if (!inDistance.value) return;

                grid.RemoveGridObject(previousGridIndex.value, e);
                grid.SetGridObjects(indexInGrid.value, e);

            }).Run();
    }
}