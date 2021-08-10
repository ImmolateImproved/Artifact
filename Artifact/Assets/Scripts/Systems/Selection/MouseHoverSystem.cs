using Unity.Entities;
using Latios;
using UnityEngine;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Mathematics;

public class MouseHoverSystem : SubSystem
{
    private BuildPhysicsWorld physicsWorld;

    protected override void OnCreate()
    {
        physicsWorld = World.GetOrCreateSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        Entities.ForEach((ref MousePosition mousePosition, in SelectionFilter selectionManager) =>
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
                var isHoverEntityExisting = TryGetSingletonEntity<Hover>(out var hoverEntity);

                if (hoverEntity != raycastHit.Entity)
                {
                    if (isHoverEntityExisting)
                    {
                        EntityManager.RemoveComponent<Hover>(hoverEntity);
                    }

                    EntityManager.AddComponentData(raycastHit.Entity, new Hover());
                }

                mousePosition.value = new float2(raycastHit.Position.x, raycastHit.Position.z);
            }

        }).WithStructuralChanges().Run();
    }
}