using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using Unity.Transforms;
using UnityEngine;

public partial class UnitSelectionSystem : SubSystem
{
    private EntityQuery selectedQuery;

    protected override void OnCreate()
    {
        selectedQuery = GetEntityQuery(typeof(Selected));
    }

    public override bool ShouldUpdateSystem()
    {
        var shouldUpdate = Input.GetMouseButtonDown(0) && sceneBlackboardEntity.HasComponent<SelectedUnit>();

        return shouldUpdate;
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<Click>()
            .ForEach((in IndexInGrid gridPosition) =>
            {
                var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

                var unit = grid.GetGridObject(gridPosition.current);

                sceneBlackboardEntity.SetComponentData(new SelectedUnit { value = unit });

                EntityManager.RemoveComponent<Selected>(selectedQuery.ToEntityArray(Allocator.Temp));

                if (EntityManager.HasComponent<Selectable>(unit))
                {
                    EntityManager.AddComponent<Selected>(unit);
                }

            }).WithStructuralChanges().Run();
    }
}