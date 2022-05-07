using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class UnitInitializationSystem : SubSystem
{
    private BeginInitializationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = ecbSystem.CreateCommandBuffer();

        var gridConfig = GetSingleton<GridConfig>();
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.WithNone<MoveRangeAssociated>().WithAll<UnitTag>()
            .ForEach((Entity entity, ref Translation translation, ref IndexInGrid gridIndex, ref PreviousGridIndex previousGridIndex) =>
            {
                var node = gridConfig.PositionToNode(translation.Value);

                gridIndex.value = node;
                previousGridIndex.value = node;

                grid.SetUnit(node, entity);

                var positionResult = grid[node];

                if (positionResult != null)
                {
                    var position = positionResult.Value;

                    translation.Value = new float3(position.x, translation.Value.y, position.y);
                }

            }).Run();

        Entities.WithNone<MoveRangeAssociated>()
            .ForEach((Entity e, in MoveRange moveRange, in UnitSelectionPointer selectionPointer) =>
            {
                var moveRangeSet = new MoveRangeSet(HexTileNeighbors.CalculateTilesCount(moveRange.value), Allocator.Persistent);
                EntityManager.AddCollectionComponent(e, moveRangeSet);

                ecb.AddComponent<Disabled>(selectionPointer.value);

            }).WithStructuralChanges().Run();
    }
}