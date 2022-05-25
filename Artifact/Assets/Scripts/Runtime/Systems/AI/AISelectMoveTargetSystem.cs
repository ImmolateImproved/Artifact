using Latios;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class AISelectMoveTargetSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var pathfinder = new PathfindingSystem.AStarPathfinding(grid);

        Entities.ForEach((ref MoveDestination moveDestination, ref IndexInGrid indexInGrid, ref WaypointsMovement waypointsMovement,
            ref DynamicBuffer<UnitPath> path, in MoveDirection moveDirection) =>
            {
                if (!moveDestination.inDistance)
                    return;

                var nextNode = grid.GetNextNode(indexInGrid.value, moveDirection.value);

                if (grid.HasNode(nextNode))
                {
                    pathfinder.FindPath(indexInGrid.value, nextNode, path.Reinterpret<int2>());
                    waypointsMovement.currentWaypointIndex = 0;
                }

            }).Run();
    }
}