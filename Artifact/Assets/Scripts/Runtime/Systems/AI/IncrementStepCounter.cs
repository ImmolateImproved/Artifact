using Latios;
using Unity.Entities;

public partial class IncrementStepCountersSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.ForEach((ref StepCounters stepCounters, ref MoveDirection moveDirection, in MoveDestination moveDestination, in IndexInGrid indexInGrid) =>
        {
            if (!moveDestination.inDistance) return;

            stepCounters.pointA++;
            stepCounters.pointB++;

            

        }).Run();
    }
}