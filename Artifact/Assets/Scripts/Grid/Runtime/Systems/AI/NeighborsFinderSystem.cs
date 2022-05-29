using Latios;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class NeighborsFinderSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.ForEach((ref DynamicBuffer<NeighborGridObjects> neighborGridObjects, in IndexInGrid indexInGrid, in InDistance inDistance, in NotificationRange notificationRange) =>
        {
            if (!inDistance.value)
                return;

            var gridObjectsInRange = new NativeList<int2>(4, Allocator.Temp);
            grid.FindGridObjects(indexInGrid.value, notificationRange.value, gridObjectsInRange);

            neighborGridObjects.Clear();

            if (gridObjectsInRange.Length == 0)
                return;

            for (int i = 0; i < gridObjectsInRange.Length; i++)
            {
                var gridObjectNode = gridObjectsInRange[i];

                var gridObjects = grid.GetGridObjects(gridObjectNode);

                foreach (var item in gridObjects)
                {
                    var objType = GetComponent<GridObjectType>(item).value;

                    neighborGridObjects.Add(new NeighborGridObjects
                    {
                        node = gridObjectNode,
                        objectType = objType
                    });
                }
            }

        }).Run();

        Entities.ForEach((ref SwarmIntelligenceData aIData, ref MoveDirection moveDirection, in DynamicBuffer<NeighborGridObjects> neighborGridObjects, in InDistance inDistance) =>
        {
            if (!inDistance.value)
                return;

            aIData.stepsCounter++;

            if (aIData.stepsCounter % 4 == 0)
            {
                moveDirection.value = AxialDirectionsExtentions.GetNextDirection(moveDirection.value, aIData.stepsCounter % 8 == 0);
            }

            for (int i = 0; i < neighborGridObjects.Length; i++)
            {
                var objectType = neighborGridObjects[i].objectType;

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
            }

        }).Run();
    }


}