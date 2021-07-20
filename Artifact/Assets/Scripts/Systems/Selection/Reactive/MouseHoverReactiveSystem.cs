using Unity.Entities;
using Latios;

public struct HoverInternal : ISystemStateComponentData
{

}

public class MouseHoverReactiveSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithAll<Hover>().WithNone<HoverInternal>()
            .ForEach((Entity e) =>
            {
                ecb.AddComponent(e, new HoverInternal());

            }).Run();

        Entities.WithAll<HoverInternal>().WithNone<Hover>()
            .ForEach((Entity e) =>
            {
                ecb.RemoveComponent<HoverInternal>(e);

            }).Run();
    }
}