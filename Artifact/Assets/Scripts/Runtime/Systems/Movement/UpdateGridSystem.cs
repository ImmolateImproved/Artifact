using Unity.Entities;
using Latios;
using UnityEngine;

public partial class UpdateGridSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.WithAll<Moving>()
            .ForEach((Entity e, ref PreviousGridIndex previousGridIndex, in IndexInGrid indexInGrid, in DynamicBuffer<UnitPath> path) =>
            {
                if (!previousGridIndex.value.Equals(indexInGrid.value))
                {
                    grid.RemoveUnit(previousGridIndex.value);

                    previousGridIndex.value = indexInGrid.value;

                    grid.SetUnit(indexInGrid.value, e);
                }

            }).Run();
    }
}