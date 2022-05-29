using Latios;
using Latios.Psyshock;
using NonGrid.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public partial class FindNeighborUnitsSystem : SubSystem
{
    private EntityQuery m_query;

    protected override void OnUpdate()
    {
        if ((UnityEngine.Time.frameCount % 25 != 0))
            return;

        var settings = sceneBlackboardEntity.GetComponentData<AISettings>();

        var sphereCollider = new SphereCollider(float3.zero, settings.searchRadius / 2);
        var bodies = new NativeArray<ColliderBody>(m_query.CalculateEntityCount(), Allocator.TempJob);

        Entities.WithAll<UnitTag>()
            .ForEach((Entity entity, int entityInQueryIndex, ref DynamicBuffer<NeighborUnit> neighborBuffer, in Translation translation) =>
            {
                neighborBuffer.Clear();

                bodies[entityInQueryIndex] = new ColliderBody
                {
                    collider = sphereCollider,
                    entity = entity,
                    transform = new RigidTransform(quaternion.identity, translation.Value)
                };

            }).WithStoreEntityQueryInField(ref m_query).ScheduleParallel();

        var сollisionLayerSettings = new CollisionLayerSettings
        {
            worldAABB = new Aabb(float3.zero, new float3(500f, 500f, 500f)),
            worldSubdivisionsPerAxis = new int3(1, 8, 8)
        };

        Dependency = Physics.BuildCollisionLayer(bodies).WithSettings(сollisionLayerSettings).ScheduleParallel(out var layer, Allocator.TempJob, Dependency);
        Dependency = bodies.Dispose(Dependency);

        var processor = new FindNeighborsProcessor
        {
            neighborBfe = GetBufferFromEntity<NeighborUnit>()
        };
        Dependency = Physics.FindPairs(layer, processor).ScheduleParallel(Dependency);
        Dependency = layer.Dispose(Dependency);
    }

    private struct FindNeighborsProcessor : IFindPairsProcessor
    {
        public PhysicsBufferFromEntity<NeighborUnit> neighborBfe;

        public void Execute(FindPairsResult result)
        {
            if (Physics.DistanceBetween(result.bodyA.collider, result.bodyA.transform, result.bodyB.collider, result.bodyB.transform, 0f, out _))
            {
                neighborBfe[result.entityA].Add(new NeighborUnit { value = new EntityWith<NotificationListener> { entity = result.entityB } });
                neighborBfe[result.entityB].Add(new NeighborUnit { value = new EntityWith<NotificationListener> { entity = result.entityA } });
            }
        }
    }
}