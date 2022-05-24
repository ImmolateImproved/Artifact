using Latios;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;

[BurstCompile]
public partial struct SelectDirectionJob : IJobEntity
{
    public Grid grid;
    public Rng rng;

    public void Execute([EntityInQueryIndex] int index, ref MoveDirection moveDirection, ref MoveDestination moveDestination, in IndexInGrid indexInGrid)
    {
        var random = rng.GetSequence(index);

        int2 nextNode;

        do
        {
            nextNode = GetNextNode(indexInGrid.value, moveDirection.value);

            if (grid.IsWalkable(nextNode))
            {
                break;
            }

            var randomDiretion = random.NextInt(0, AxialDirectionsExtentions.DIRECTIONS_COUNT);
            moveDirection.value = (AxialDirections)randomDiretion;// moveDirection.value.ReverseDirection();

        } while (true);

        moveDestination.node = nextNode;
    }

    private int2 GetNextNode(int2 currentNode, AxialDirections direction)
    {
        var dir = grid.neighbors[(int)direction];

        var nextNode = currentNode + dir;

        return nextNode;
    }
}

public partial class AISelectDirectionSystem : SubSystem
{
    private Rng rng;

    public override void OnNewScene()
    {
        rng = new Rng("AIMovementSystem");
    }

    protected override void OnUpdate()
    {
        var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        new SelectDirectionJob
        {
            grid = grid,
            rng = rng

        }.Schedule();

        rng.Shuffle();
    }
}