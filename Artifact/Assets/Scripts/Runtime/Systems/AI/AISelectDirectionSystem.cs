using Latios;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[BurstCompile]
public partial struct SelectDirectionJob : IJobEntity
{
    public Grid grid;
    public Rng rng;

    [ReadOnly]
    public ComponentDataFromEntity<GridObjectType> objectTypeCDFE;

    public EntityManager entityManager;

    public void Execute([EntityInQueryIndex] int index, ref SwarmIntelligenceData aIData, ref MoveDirection moveDirection, in IndexInGrid indexInGrid, in MoveDestination moveDestination)
    {
        if (!moveDestination.inDistance) return;

        var nextNode = grid.GetNextNode(indexInGrid.value, moveDirection.value);

        while (!grid.HasNode(nextNode))
        {
            var random = rng.GetSequence(index);
            rng.Shuffle();
            var randomDirection = random.NextInt(0, AxialDirectionsExtentions.DIRECTIONS_COUNT);
            moveDirection.value = (AxialDirections)randomDirection;

            nextNode = grid.GetNextNode(indexInGrid.value, moveDirection.value);
        }

        var gridObjectsInRange = new NativeList<Entity>(5, Allocator.Temp);
        grid.FindGridObjects(indexInGrid.value, aIData.notificationRange, gridObjectsInRange);

        if (gridObjectsInRange.Length == 0)
            return;

        var gridObjects = gridObjectsInRange;

        var objectType = default(GridObjectTypes);

        for (int i = 0; i < gridObjects.Length; i++)
        {
            var obj = gridObjects[i];
            objectType = objectTypeCDFE[obj].value;

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
    }
}

public partial class AISelectDirectionSystem : SubSystem
{
    private Rng rng;

    public override void OnNewScene()
    {
        rng = new Rng("AIMovementSystem");
    }

    protected override void OnUpdate()
    {
        var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);
        Dependency.Complete();

        var objectTypeCDFE = GetComponentDataFromEntity<GridObjectType>(true);

        new SelectDirectionJob
        {
            grid = grid,
            rng = rng,
            objectTypeCDFE = objectTypeCDFE,
            entityManager = EntityManager

        }.Run();

        rng.Shuffle();
    }
}