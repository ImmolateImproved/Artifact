using Latios;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

public partial class AINotifierSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var objectTypeCDFE = GetComponentDataFromEntity<GridObjectType>(true);

        Entities.ForEach((ref SwarmIntelligenceData aiData, in MoveDestination moveDestination, in IndexInGrid indexInGrid) =>
        {
            if (!moveDestination.inDistance) return;

            aiData.stepsToBase++;
            aiData.stepsToResource++;

            var unitsInRange = new NativeList<Entity>(5, Allocator.Temp);
            grid.FindGridObjects(indexInGrid.value, aiData.notificationRange, unitsInRange);

            for (int i = 0; i < unitsInRange.Length; i++)
            {
                var otherUnit = unitsInRange[i];

                if (objectTypeCDFE.TryGetComponent(otherUnit, out var objType))
                {
                    if (objType.value != GridObjectTypes.Unit)
                        continue;
                }

                var newStepsToBase = aiData.stepsToBase + aiData.notificationRange;
                var newStepsToResource = aiData.stepsToResource + aiData.notificationRange;

                var otherAIData = GetComponent<NotificationListener>(otherUnit);

                if (otherAIData.stepsToBase > newStepsToBase)
                {
                    otherAIData.stepsToBase = newStepsToBase;
                    otherAIData.notifierNode = indexInGrid.value;
                    otherAIData.changed = true;
                }
                if (otherAIData.stepsToResource > newStepsToResource)
                {
                    otherAIData.stepsToResource = newStepsToResource;
                    otherAIData.notifierNode = indexInGrid.value;
                    otherAIData.changed = true;
                }

                SetComponent(otherUnit, otherAIData);
            }

            unitsInRange.Dispose();

        }).Run();
    }
}