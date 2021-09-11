using Unity.Entities;
using Latios;

public struct SelectedInternal : ISystemStateComponentData
{

}

public class UnitSelectionReactiveSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((Entity e) =>
            {
                ecb.RemoveComponent<SelectedInternal>(e);

            }).Run();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                ecb.AddComponent<SelectedInternal>(e);

            }).Run();
    }
}