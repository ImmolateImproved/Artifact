using Latios;
using Unity.Entities;

public partial class RemoveDeadSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateDestroyCommandBuffer();

        Entities.ForEach((Entity e, in AliveStatus aliveStatus) =>
        {
            if (!aliveStatus.isAlive)
            {
                ecb.Add(e);
            }

        }).Run();
    }
}