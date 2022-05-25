using Latios;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class AIListenerSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var pathfinder = new PathfindingSystem.AStarPathfinding(grid);

        Entities.ForEach((ref MoveDirection moveDirection, ref SwarmIntelligenceData aIData, ref NotificationListener notificationListener, ref DynamicBuffer<UnitPath> path, ref WaypointsMovement waypointsMovement, in IndexInGrid indexInGrid) =>
        {
            if (!notificationListener.changed) return;

            notificationListener.changed = false;

            if (aIData.stepsToBase > notificationListener.stepsToBase)
            {
                aIData.stepsToBase = notificationListener.stepsToBase;

                if (aIData.target == GridObjectTypes.Base)
                {
                    pathfinder.FindPath(indexInGrid.value, notificationListener.notifierNode, path.Reinterpret<int2>());
                    waypointsMovement.currentWaypointIndex = 0;
                    //moveDirection.value = AxialDirectionsExtentions.FromVector(indexInGrid.value, notificationListener.notifierNode, grid.neighbors);
                }
            }

            if (aIData.stepsToResource > notificationListener.stepsToResource)
            {
                aIData.stepsToResource = notificationListener.stepsToResource;

                if (aIData.target == GridObjectTypes.Recource)
                {
                    pathfinder.FindPath(indexInGrid.value, notificationListener.notifierNode, path.Reinterpret<int2>());
                    waypointsMovement.currentWaypointIndex = 0;
                    //moveDirection.value = AxialDirectionsExtentions.FromVector(indexInGrid.value, notificationListener.notifierNode, grid.neighbors);
                }
            }

        }).Run();
    }
}