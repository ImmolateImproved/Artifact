using Latios;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class AIObserverSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.ForEach((ref SwarmIntelligenceData aIData, ref MoveDirection moveDirection, in IndexInGrid indexInGrid, in InDistance inDistance) =>
        {
            if (!inDistance.value)
                return;

            var gridObjectsInRange = new NativeList<Entity>(5, Allocator.Temp);
            grid.FindGridObjects(indexInGrid.value, aIData.notificationRange, gridObjectsInRange);

            if (gridObjectsInRange.Length == 0)
                return;

            var gridObjects = gridObjectsInRange;

            var objectType = GridObjectTypes.Unit;

            for (int i = 0; i < gridObjects.Length; i++)
            {
                var objType = GetComponent<GridObjectType>(gridObjects[i]).value;

                if (objType != GridObjectTypes.Unit)
                {
                    objectType = objType;
                }
            }

            if (objectType == GridObjectTypes.Base)
            {
                aIData.stepsToBase = 0;

                if (aIData.target == GridObjectTypes.Base)
                {
                    moveDirection.value = moveDirection.value.ReverseDirection();

                    aIData.target = GridObjectTypes.Recource;
                }
            }

            if (objectType == GridObjectTypes.Recource)
            {
                aIData.stepsToResource = 0;

                if (aIData.target == GridObjectTypes.Recource)
                {
                    moveDirection.value = moveDirection.value.ReverseDirection();
                    aIData.target = GridObjectTypes.Base;
                }
            }

        }).Run();
    }
}