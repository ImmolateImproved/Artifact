using Latios;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

public class ClickSystem : SubSystem
{
    private EndSimulationEntityCommandBufferSystem ecbSystem;

    protected override void OnCreate()
    {
        ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        Entities.WithAll<Hover>()
            .ForEach((ref Entity e) =>
            {
                var ecb = ecbSystem.CreateCommandBuffer();

                EntityManager.AddComponentData(e, new Click());
                ecb.RemoveComponent<Click>(e);

            }).WithStructuralChanges().Run();

    }
}