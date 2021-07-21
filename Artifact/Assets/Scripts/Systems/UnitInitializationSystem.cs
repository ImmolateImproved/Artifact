using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;

public class UnitInitializationSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.WithNone<MoveRangeAssociated>()
            .ForEach((Entity e, in MoveRange moveRange) =>
            {
                var moveRangeSet = new MoveRangeSet(HexTileNeighbors.CalculateTilesCount(moveRange.value, 6), Allocator.Persistent);
                EntityManager.AddCollectionComponent(e, moveRangeSet);

            }).WithStructuralChanges().Run();
    }
}
