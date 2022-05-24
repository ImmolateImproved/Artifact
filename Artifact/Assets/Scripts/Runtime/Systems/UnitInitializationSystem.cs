using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class UnitInitializationSystem : SubSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GridInitialized>();
    }

    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        var gridConfig = GetSingleton<GridConfig>();
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.WithAll<UnitTag>().WithNone<UnitInitialized>()
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

                ecb.AddComponent<UnitInitialized>(entity);

            }).Run();

        Entities.WithAll<UnitTag>().WithNone<UnitInitialized>()
            .ForEach((in UnitSelectionPointer selectionPointer)=>
            {
                ecb.AddComponent<Disabled>(selectionPointer.value);

            }).Run();
    }
}