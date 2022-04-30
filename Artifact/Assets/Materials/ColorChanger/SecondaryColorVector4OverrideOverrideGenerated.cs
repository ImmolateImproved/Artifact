using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_SecondaryColor", MaterialPropertyFormat.Float4)]
    struct SecondaryColorVector4Override : IComponentData
    {
        public float4 Value;
    }
}
