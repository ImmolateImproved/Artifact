using Latios;
using Unity.Entities;
using Unity.Mathematics;

public partial class SelectDestinationSystem : SubSystem
{
    private Rng m_rng;

    public override void OnNewScene()
    {
        m_rng = new Rng("SelectDestinationSystem");
    }

    protected override void OnUpdate()
    {
        var rng = m_rng.Shuffle();

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.ForEach((int entityInQueryIndex, ref DestinationNode destination, ref MoveDirection moveDirection, in IndexInGrid indexInGrid, in InDistance inDistance) =>
        {
            if (!inDistance.value)
                return;

            var random = rng.GetSequence(entityInQueryIndex);

            var attemptsCount = AxialDirectionsExtentions.DIRECTIONS_COUNT;

            while (true)
            {
                var nextNode = grid.GetNeighborNodeFromDirection(indexInGrid.current, moveDirection.value);

                if (grid.HasNode(nextNode))
                {
                    destination.node = nextNode;
                    destination.position = grid.GetNodePosition(nextNode);
                    return;
                }

                moveDirection.value = AxialDirectionsExtentions.GetRandomDirection(random);

                attemptsCount--;
                if (attemptsCount <= 0)
                {
                    return;
                }
            }

        }).Run();
    }
}