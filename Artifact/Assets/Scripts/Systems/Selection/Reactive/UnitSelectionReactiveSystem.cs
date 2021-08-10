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

        var selectedUnit = default(SelectedUnit);

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((Entity e) =>
            {
                ecb.RemoveComponent<SelectedInternal>(e);
                selectedUnit.value = Entity.Null;

            }).Run();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                ecb.AddComponent<SelectedInternal>(e);
                selectedUnit.value = e;

            }).Run();

        sceneBlackboardEntity.SetComponentData(selectedUnit);
    }
}