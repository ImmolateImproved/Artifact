using Unity.Entities;
using Latios;
using UnityEngine;
using Unity.Rendering;

public class MouseHoverViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = sceneBlackboardEntity.GetComponentData<HoverColor>();

        Entities.WithAll<Hover>().WithNone<HoverInternal>()
            .ForEach((ref URPMaterialPropertyBaseColor color) =>
            {
                color.Value = (Vector4)unitColors.value;

            }).Run();

        Entities.WithAll<HoverInternal>().WithNone<Hover>()
            .ForEach((Entity e, ref URPMaterialPropertyBaseColor color, in DefaultColor entityColors) =>
            {
                color.Value = (Vector4)entityColors.defaultColor;

            }).Run();
    }
}