using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using UnityEngine;

public class UnitInitializationSystem : SubSystem
{
    private BeginInitializationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer();

        Entities.WithNone<MoveRangeAssociated>()
            .ForEach((Entity e, in MoveRange moveRange, in UnitSelectionPointer selectionPointer) =>
            {
                var moveRangeSet = new MoveRangeSet(HexTileNeighbors.CalculateTilesCount(moveRange.value, 6), Allocator.Persistent);
                EntityManager.AddCollectionComponent(e, moveRangeSet);

                ecb.AddComponent<Disabled>(selectionPointer.value);

            }).WithStructuralChanges().Run();
    }
}