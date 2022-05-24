using Latios;
using Unity.Entities;
using UnityEngine;

public partial class UpdateGridSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.WithAll<Moving>()
            .ForEach((Entity e, ref IndexInGrid indexInGrid, in PreviousGridIndex previousGridIndex, in MoveDestination moveDestination) =>
            {
                if (!moveDestination.inDistance) return;

                grid.RemoveUnit(previousGridIndex.value);
                grid.SetUnit(indexInGrid.value, e);

            }).Run();
    }
}