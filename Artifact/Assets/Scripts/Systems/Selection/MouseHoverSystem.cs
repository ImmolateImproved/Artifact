using Unity.Entities;
using Latios;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Collections;
using Unity.Rendering;

public class MouseHoverSystem : SubSystem
{
    private EntityQuery hoverQuery;
    private BuildPhysicsWorld physicsWorld;

    protected override void OnCreate()
    {
        hoverQuery = GetEntityQuery(typeof(Hover));
        physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((in SelectionManager selectionManager) =>
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            var rayInput = new RaycastInput
            {
                Start = ray.origin,
                End = ray.origin + ray.direction * 100,
                Filter = selectionManager.collisionFilter
            };

            if (physicsWorld.PhysicsWorld.CollisionWorld.CastRay(rayInput, out var raycastHit))
            {
                var isHoverEntityExisting = TryGetSingletonEntity<Hover>(out var hoverEntity);//hoverQuery.ToEntityArray(Allocator.Temp);

                if (hoverEntity != raycastHit.Entity)
                {
                    if (isHoverEntityExisting)
                    {
                        EntityManager.RemoveComponent<Hover>(hoverEntity);
                    }

                    EntityManager.AddComponentData(raycastHit.Entity, new Hover());
                }
            }

        }).WithStructuralChanges().Run();
    }
}