using Latios;
using Unity.Entities;
using UnityEngine;

public partial class GridAINotifierSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var objectTypeCDFE = GetComponentDataFromEntity<GridObjectType>(true);

        Entities.ForEach((ref SwarmIntelligenceData aiData, in InDistance inDistance) =>
        {
            if (!inDistance.value) return;

            aiData.stepsToBase++;
            aiData.stepsToResource++;

        }).Run();

        var swarmIntelligenceDataCDFE = GetComponentDataFromEntity<SwarmIntelligenceData>();

        Entities.ForEach((in SwarmIntelligenceData aiData, in NotificationRange notificationRange, in InDistance inDistance,
            in IndexInGrid indexInGrid, in DynamicBuffer<NeighborGridObjects> neighborUnits) =>
        {
            for (int i = 0; i < neighborUnits.Length; i++)
            {
                var gridObjectsIterator = grid.GetGridObjects(neighborUnits[i].node);

                foreach (var otherUnit in gridObjectsIterator)
                {
                    if (objectTypeCDFE.TryGetComponent(otherUnit, out var objType))
                    {
                        if (objType.value != GridObjectTypes.Unit)
                            continue;
                    }

                    var newStepsToBase = aiData.stepsToBase;
                    var newStepsToResource = aiData.stepsToResource;

                    var otherAIData = swarmIntelligenceDataCDFE[otherUnit];//GetComponent<SwarmIntelligenceData>(otherUnit);
                    var otherAI = GetComponent<NotificationListener>(otherUnit);
                    //Debug.Log($"otherAIData.stepsToBase {otherAIData.stepsToBase} newStepsToBase {newStepsToBase}");
                    if (otherAIData.stepsToBase > newStepsToBase)
                    {
                        otherAIData.stepsToBase = newStepsToBase;
                        otherAI.notifierNode = indexInGrid.value;
                        otherAI.changed = true;
                    }
                    if (otherAIData.stepsToResource > newStepsToResource)
                    {
                        otherAIData.stepsToResource = newStepsToResource;
                        otherAI.notifierNode = indexInGrid.value;
                        otherAI.changed = true;
                    }
                    swarmIntelligenceDataCDFE[otherUnit] = otherAIData;
                    //SetComponent(otherUnit, otherAIData);
                    SetComponent(otherUnit, otherAI);
                }

            }

        }).Run();
    }
}