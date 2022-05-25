using Latios;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class SelectMoveTargetSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var pathfinder = new PathfindingSystem.AStarPathfinding(grid);

        Entities.WithAll<Moving>()
          .ForEach((ref MoveDestination moveDestination, ref IndexInGrid indexInGrid, ref WaypointsMovement waypointsMovement,
                   ref DynamicBuffer<UnitPath> path, ref MoveDirection moveDirection) =>
                   {
                       var currentDirection = moveDirection.value;

                       var attemptsCount = AxialDirectionsExtentions.DIRECTIONS_COUNT;

                       while (path.Length == 0)
                       {
                           moveDirection.value = currentDirection;

                           var nextNode = grid.GetNextNode(indexInGrid.value, moveDirection.value);

                           if (grid.HasGridOject(nextNode))
                           {
                               var gridObjs = grid.GetGridObjects(nextNode);

                               foreach (var item in gridObjs)
                               {
                                   if (GetComponent<GridObjectType>(item).value != GridObjectTypes.Unit)
                                       return;
                               }
                           }

                           if (grid.HasNode(nextNode))
                           { 
                               pathfinder.FindPath(indexInGrid.value, nextNode, path.Reinterpret<int2>());
                               waypointsMovement.currentWaypointIndex = 0;
                           }

                           currentDirection = AxialDirectionsExtentions.GetNextDirection(currentDirection);
                           attemptsCount--;

                           if (attemptsCount <= 0)
                               return;

                       }

                       moveDestination.node = path[waypointsMovement.currentWaypointIndex].nodeIndex;

                   }).Run();
    }
}