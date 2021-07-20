using Unity.Entities;
using Unity.Rendering;
using Latios;

public struct SelectedInternal : ISystemStateComponentData
{

}

public class UnitSelectionReactiveSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = latiosWorld.sceneBlackboardEntity.GetComponentData<SelectionColors>();

        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                ecb.AddComponent<SelectedInternal>(e);

            }).Run();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((Entity e) =>
            {
                ecb.RemoveComponent<SelectedInternal>(e);

            }).Run();
    }
}