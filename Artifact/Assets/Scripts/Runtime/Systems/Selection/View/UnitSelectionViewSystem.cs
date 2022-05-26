using Unity.Entities;
using Latios;

public partial class UnitSelectionViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = sceneBlackboardEntity.GetComponentData<HoverColor>();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach(( in UnitSelectionPointer selectionPointer) =>
            {
                EntityManager.SetEnabled(selectionPointer.value, true);

            }).WithStructuralChanges().Run();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((in UnitSelectionPointer selectionPointer) =>
            {
                EntityManager.SetEnabled(selectionPointer.value, false);


            }).WithStructuralChanges().Run();
    }
}