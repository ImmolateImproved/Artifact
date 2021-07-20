using Unity.Entities;
using Latios;
using UnityEngine;
using Unity.Rendering;

public class MouseHoverViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var unitColors = sceneBlackboardEntity.GetComponentData<SelectionColors>();

        Entities.WithAll<Hover>().WithNone<HoverInternal>()
            .ForEach((ref URPMaterialPropertyBaseColor color) =>
            {
                color.Value = (Vector4)unitColors.hoveredColor;

            }).Run();

        Entities.WithAll<HoverInternal>().WithNone<Hover>()
            .ForEach((Entity e, ref URPMaterialPropertyBaseColor color, in EntityColors entityColors) =>
            {
                var newColor = (Vector4)entityColors.defaultColor;

                if (HasComponent<Selected>(e))
                    newColor = unitColors.selectedColor;

                color.Value = newColor;


            }).Run();
    }
}