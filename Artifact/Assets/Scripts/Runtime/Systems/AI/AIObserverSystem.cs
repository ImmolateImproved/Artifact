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

        Entities.ForEach((ref SwarmIntelligenceData aIData, ref MoveDirection moveDirection, in IndexInGrid indexInGrid, in MoveDestination moveDestination) =>
        {
            if (!moveDestination.inDistance) return;

            var gridObjectsInRange = new NativeList<Entity>(5, Allocator.Temp);
            grid.FindGridObjects(indexInGrid.value, aIData.notificationRange, gridObjectsInRange);

            if (gridObjectsInRange.Length == 0)
                return;

            var gridObjects = gridObjectsInRange;

            var objectType = default(GridObjectTypes);

            for (int i = 0; i < gridObjects.Length; i++)
            {
                var obj = gridObjects[i];
                objectType = GetComponent<GridObjectType>(obj).value;

                if (objectType == GridObjectTypes.Unit)
                    continue;
            }

            if (objectType == GridObjectTypes.Unit)
                return;

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