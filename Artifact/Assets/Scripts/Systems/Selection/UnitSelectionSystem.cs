using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;

public class UnitSelectionSystem : SubSystem
{
    private EntityQuery selectedQuery;

    protected override void OnCreate()
    {
        selectedQuery = GetEntityQuery(typeof(Selected));
    }

    public override bool ShouldUpdateSystem()
    {
        return Input.GetMouseButtonDown(0);
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<Click>()
            .ForEach((in IndexInGrid gridPosition) =>
            {
                var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

                var unit = grid.GetUnit(gridPosition.value);

                EntityManager.RemoveComponent<Selected>(selectedQuery.ToEntityArray(Allocator.Temp));

                if (EntityManager.HasComponent<Selectable>(unit))
                {
                    EntityManager.AddComponentData(unit, new Selected());
                }


            }).WithStructuralChanges().Run();
    }
}