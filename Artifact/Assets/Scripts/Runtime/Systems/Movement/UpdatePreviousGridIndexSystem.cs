using Latios;
using Unity.Entities;

public partial class UpdatePreviousGridIndexSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.WithAll<Moving>()
            .ForEach((ref MoveDestination moveDestination, ref PreviousGridIndex previousGridIndex, in IndexInGrid indexInGrid) =>
            {
                if (!moveDestination.inDistance) return;

                previousGridIndex.value = indexInGrid.value;
                moveDestination.inDistance = false;

            }).Run();
    }
}