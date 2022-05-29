using Latios;
using Unity.Entities;
using UnityEngine;

public partial class SelectDestinationSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.WithAll<Moving>()
            .ForEach((ref DestinationNode destinationNode, ref IndexInGrid indexInGrid, ref MoveDirection moveDirection,
                     ref PathfindingTarget pathfindingTarget, in InDistance inDistance, in DynamicBuffer<UnitPath> path) =>
                     {
                         if (!inDistance.value || path.Length != 0)
                             return;

                         pathfindingTarget.pathNeeded = pathfindingTarget.pathNeeded && (Grid.Distance(indexInGrid.value, pathfindingTarget.node) > 1);
                         
                         if (pathfindingTarget.pathNeeded)
                             return;

                         var attemptsCount = AxialDirectionsExtentions.DIRECTIONS_COUNT;

                         //find walkable neighbor node
                         while (true)
                         {
                             var neighborNode = grid.GetNeighborNode(indexInGrid.value, moveDirection.value);

                             if (grid.HasNode(neighborNode))
                             {
                                 destinationNode.node = neighborNode;
                                 destinationNode.position = grid.GetNodePosition(neighborNode);
                                 return;
                             }

                             moveDirection.value = moveDirection.value.ReverseDirection();//AxialDirectionsExtentions.GetNextDirection(moveDirection.value);

                             attemptsCount--;

                             if (attemptsCount <= 0)
                                 return;

                         }

                     }).Run();
    }
}
