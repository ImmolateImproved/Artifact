using Unity.Entities;
using Latios;
using UnityEngine;
using Unity.Rendering;

public partial class MouseHoverViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var hoverColor = sceneBlackboardEntity.GetComponentData<HoverColor>();

        Entities.WithAll<Hover>().WithNone<HoverInternal>()
            .ForEach((ref GlowColorVector4Override color) =>
            {
                color.Value = (Vector4)hoverColor.value;

            }).Run();

        Entities.WithAll<HoverInternal>().WithNone<Hover>()
            .ForEach((Entity e, ref GlowColorVector4Override color) =>
            {
                color.Value = (Vector4)Color.black;

            }).Run();

    }
}