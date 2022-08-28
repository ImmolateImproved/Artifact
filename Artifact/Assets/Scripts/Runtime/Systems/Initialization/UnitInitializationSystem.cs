using Latios;
using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using UnityEngine;

public partial class UnitInitializationSystem : SubSystem
{
    public EntityQuery nonInitializedUnitsQuery;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GridInitializedTag>();

        RequireForUpdate(nonInitializedUnitsQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        var gridConfig = sceneBlackboardEntity.GetComponentData<GridConfig>();
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.ForEach((in UnitSelectionPointer unitSelectionPointer) =>
        {
            var selectionPointerPosition = EntityManager.GetComponentData<Translation>(unitSelectionPointer.value);
            selectionPointerPosition.Value.y = -0.5f;

            EntityManager.SetEnabled(unitSelectionPointer.value, false);

        }).WithStructuralChanges().Run();

        Entities.WithAll<UnitTag>().WithNone<UnitInitialized>()
            .ForEach((Entity entity, ref Translation translation, ref IndexInGrid gridIndex) =>
            {
                var node = gridConfig.PositionToNode(translation.Value);

                if (grid.IsWalkable(node))
                {
                    gridIndex.current = node;

                    grid.SetGridObject(node, entity);

                    var position = grid.GetNodePosition(node);
                    position.y = translation.Value.y;

                    translation.Value = position;

                    ecb.AddComponent<UnitInitialized>(entity);
                }

            }).WithStoreEntityQueryInField(ref nonInitializedUnitsQuery).Run();
    }
}
