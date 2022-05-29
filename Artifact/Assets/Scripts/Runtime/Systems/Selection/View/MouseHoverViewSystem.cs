using Unity.Entities;
using Latios;
using UnityEngine;
using Unity.Rendering;

public partial class MouseHoverViewSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.WithChangeFilter<HoverTile>()
            .ForEach((in HoverTile hoverTile,in HoverColor hoverColor) =>
            {
                if (hoverTile.current != hoverTile.previous)
                {
                    if (hoverTile.current != Entity.Null)
                    { 
                        SetComponent(hoverTile.current, new GlowColorVector4Override { Value = (Vector4)hoverColor.value });
                    }
                    if (hoverTile.previous != Entity.Null)
                    {
                        SetComponent(hoverTile.previous, new GlowColorVector4Override { Value = (Vector4)Color.black });
                    }
                }

            }).Run();
    }
}