using Latios;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

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

        Entities.ForEach((Entity e, int entityInQueryIndex, ref DestinationNode destination, ref MoveDirection moveDirection, in IndexInGrid indexInGrid, in AliveStatus aliveStatus) =>
        {
            if (!aliveStatus.isAlive)
                return;

            var random = rng.GetSequence(entityInQueryIndex);

            var attemptsCount = HexDirectionsExtentions.DIRECTIONS_COUNT;

            while (true)
            {
                moveDirection.value = HexDirectionsExtentions.GetRandomDirection(ref random);

                var nextNode = grid.GetNeighborNodeFromDirection(indexInGrid.current, moveDirection.value);

                if (grid.IsWalkable(nextNode))
                {
                    destination.node = nextNode;
                    destination.position = grid.GetNodePosition(nextNode);

                    grid.RemoveGridObject(indexInGrid.current);
                    grid.SetGridObject(nextNode, e);

                    return;
                }

                attemptsCount--;
                if (attemptsCount <= 0)
                {
                    break;
                }
            }

            //destination.node = indexInGrid.current;
            //destination.position = grid.GetNodePosition(destination.node);

            //grid.RemoveGridObject(indexInGrid.current);
            //grid.SetGridObject(destination.node, e);

        }).Run();
    }
}