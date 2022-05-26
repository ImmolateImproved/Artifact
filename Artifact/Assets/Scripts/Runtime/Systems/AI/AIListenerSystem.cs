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

        Entities.ForEach((ref SwarmIntelligenceData aIData, ref NotificationListener notificationListener, ref PathfindingTarget pathfindingTarget) =>
        {
            if (!notificationListener.changed) return;

            notificationListener.changed = false;

            if (aIData.stepsToBase > notificationListener.stepsToBase)
            {
                aIData.stepsToBase = notificationListener.stepsToBase;

                if (aIData.target == GridObjectTypes.Base)
                {
                    pathfindingTarget.node = notificationListener.notifierNode;
                }
            }

            if (aIData.stepsToResource > notificationListener.stepsToResource)
            {
                aIData.stepsToResource = notificationListener.stepsToResource;

                if (aIData.target == GridObjectTypes.Recource)
                {
                    pathfindingTarget.node = notificationListener.notifierNode;
                }
            }

        }).Run();
    }
}