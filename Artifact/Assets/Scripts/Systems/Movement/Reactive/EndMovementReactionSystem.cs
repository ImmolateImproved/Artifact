using Latios;
using Unity.Entities;

public struct Movinginternal : ISystemStateComponentData
{

}


public class EndMovementReactionSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithAll<Moving>().WithNone<Movinginternal>()
            .ForEach((Entity e) =>
            {
                ecb.AddComponent<Movinginternal>(e);

            }).Run();

        Entities.WithAll<Movinginternal>().WithNone<Moving>()
            .ForEach((Entity e) =>
            {
                ecb.RemoveComponent<Movinginternal>(e);

            }).Run();
    }
}