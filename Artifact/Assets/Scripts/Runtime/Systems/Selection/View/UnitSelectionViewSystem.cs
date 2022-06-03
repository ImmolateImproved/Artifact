using Unity.Entities;
using Latios;
using UnityEngine;
using Unity.Transforms;
using Unity.Mathematics;

public partial class UnitSelectionViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = sceneBlackboardEntity.GetComponentData<HoverColor>();

        var selectionPointer = sceneBlackboardEntity.GetComponentData<UnitSelectionPointer>();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach(() =>
            {
                EntityManager.SetEnabled(selectionPointer.value, false);

            }).WithStructuralChanges().Run();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((Entity e) =>
            {
                var selectedUnitPosition = float3.zero;
                selectedUnitPosition.y = selectionPointer.yPosition;

                EntityManager.SetComponentData(selectionPointer.value, new Parent { Value = e });
                EntityManager.SetComponentData(selectionPointer.value, new Translation { Value = selectedUnitPosition });
                EntityManager.SetEnabled(selectionPointer.value, true);

            }).WithStructuralChanges().Run();
    }
}