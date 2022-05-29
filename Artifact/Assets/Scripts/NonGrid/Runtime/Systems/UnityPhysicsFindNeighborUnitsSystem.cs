using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using NonGrid.Components;
using Unity.Physics.Systems;
using Unity.Physics;

public partial class UnityPhysicsFindNeighborUnitsSystem : SubSystem
{
    private BuildPhysicsWorld buildPhysicsWorld;

    protected override void OnCreate()
    {
        buildPhysicsWorld = World.GetExistingSystem<BuildPhysicsWorld>();
    }

    protected override void OnUpdate()
    {
        var world = buildPhysicsWorld.PhysicsWorld;

        var settings = sceneBlackboardEntity.GetComponentData<AISettings>();

        Entities.ForEach((ref DynamicBuffer<NeighborUnit> neighborUnits, in Translation translation) =>
        {
            neighborUnits.Clear();

            var hits = new NativeList<DistanceHit>(10, Allocator.Temp);
            if (world.OverlapSphere(translation.Value, settings.searchRadius, ref hits, settings.collisionFilter))
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    var entity = hits[i].Entity;

                    if (HasComponent<UnitTag>(entity))
                    {
                        var neighborUnit = new NeighborUnit
                        {
                            value = entity
                        };

                        neighborUnits.Add(neighborUnit);
                    }
                }
            }

        }).Schedule();
    }
}