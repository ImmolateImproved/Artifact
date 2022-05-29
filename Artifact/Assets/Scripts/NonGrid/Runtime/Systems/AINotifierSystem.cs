using Latios;
using NonGrid.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class AINotifierSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((in AIData aIData, in DynamicBuffer<NeighborUnit> neighbors, in Translation translation) =>
        {
            if (neighbors.Length == 0)
                return;

            var neighborsArray = neighbors.AsNativeArray();

            foreach (var unit in neighborsArray)
            {
                var otherAIData = GetComponent<AIData>(unit.value);
                var otherListener = GetComponent<NotificationListener>(unit.value);

                otherListener.stepsToBase = otherAIData.stepsToBase;
                otherListener.stepsToResource = otherAIData.stepsToResource;

                otherListener.stepsToBase = math.min(otherListener.stepsToBase, aIData.stepsToBase);
                otherListener.stepsToResource = math.min(otherListener.stepsToResource, aIData.stepsToResource);

                if (otherAIData.target == ObjectTypes.Base && otherListener.stepsToBase != otherAIData.stepsToBase)
                {
                    if (aIData.target == ObjectTypes.Base)
                    {
                        otherListener.notifierPosition = translation.Value;
                        otherListener.changed = true;
                    }
                }

                if (otherAIData.target == ObjectTypes.Resource && otherListener.stepsToResource != otherAIData.stepsToResource)
                {
                    if (aIData.target == ObjectTypes.Resource)
                    {
                        otherListener.notifierPosition = translation.Value;
                        otherListener.changed = true;
                    }
                }

                SetComponent(unit.value, otherListener);
            }

        }).Schedule();
    }
}