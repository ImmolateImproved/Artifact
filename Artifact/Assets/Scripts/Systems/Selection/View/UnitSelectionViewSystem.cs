using Unity.Entities;
using UnityEngine;
using Unity.Rendering;
using Latios;

public class UnitSelectionViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = sceneBlackboardEntity.GetComponentData<SelectionColors>();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((ref URPMaterialPropertyBaseColor color) =>
            {
                color.Value = (Vector4)unitColors.selectedColor;

            }).Run();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach((ref URPMaterialPropertyBaseColor color, in EntityColors defaultColor) =>
            {
                color.Value = (Vector4)defaultColor.defaultColor;

            }).Run();
    }
}