using Latios;
using Unity.Entities;
using UnityEngine;

public partial class UpdateGridSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.WithAll<Moving>()
            .ForEach((Entity e, in IndexInGrid indexInGrid, in InDistance inDistance) =>
            {
                if (!inDistance.value) return;

                grid.RemoveGridObject(indexInGrid.previous, e);
                grid.SetGridObjects(indexInGrid.current, e);

            }).Run();
    }
}