using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Latios;
using Unity.Transforms;

public class UnitSelectionViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = sceneBlackboardEntity.GetComponentData<SelectionColors>();

        var unitUi = this.GetSingleton<UnitUi>();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((UnitCombat unitCombat, in UnitSelectionPointer selectionPointer) =>
            {
                EntityManager.SetEnabled(selectionPointer.value, true);

                unitUi.Init(unitCombat);

            }).WithStructuralChanges().Run();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((UnitCombat unitCombat, in UnitSelectionPointer selectionPointer) =>
            {
                EntityManager.SetEnabled(selectionPointer.value, false);

                unitUi.Reset(unitCombat);

            }).WithStructuralChanges().Run();
    }
}