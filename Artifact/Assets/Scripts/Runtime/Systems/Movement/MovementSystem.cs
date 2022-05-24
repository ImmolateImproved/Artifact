using Latios;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[BurstCompile]
[WithAll(typeof(Moving))]
public partial struct MovementJob : IJobEntity
{
    public Grid grid;
    public float dt;

    public void Execute(ref Translation translation, ref MoveDestination moveDestination, ref IndexInGrid indexInGrid, in MoveSpeed moveSpeed)
    {
        var nextPosition = GetNextNodePosition(moveDestination.node, translation.Value.y);

        translation.Value = MovementUtils.MoveTowards(translation.Value, nextPosition, moveSpeed.value * dt, out var distance);

        if (distance < 0.01f)
        {
            moveDestination.inDistance = true;
            indexInGrid.value = moveDestination.node;
        }
    }

    private float3 GetNextNodePosition(int2 nextNode, float yValue)
    {
        var nextNodePosition = grid[nextNode].Value;

        var nextPosition = new float3(nextNodePosition.x, yValue, nextNodePosition.y);

        return nextPosition;
    }
}

public partial class MovementSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        new MovementJob
        {
            grid = grid,
            dt = Time.DeltaTime

        }.Schedule();
    }
}