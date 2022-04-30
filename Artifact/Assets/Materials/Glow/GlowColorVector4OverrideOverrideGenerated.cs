using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_GlowColor", MaterialPropertyFormat.Float4)]
    struct GlowColorVector4Override : IComponentData
    {
        public float4 Value;
    }
}
