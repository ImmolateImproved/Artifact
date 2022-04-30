using Unity.Entities;
using Unity.Mathematics;

namespace Unity.Rendering
{
    [MaterialProperty("_GlowPower", MaterialPropertyFormat.Float)]
    struct GlowPowerFloatOverride : IComponentData
    {
        public float Value;
    }
}
