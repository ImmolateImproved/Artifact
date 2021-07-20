using Unity.Entities;
using Unity.Mathematics;

public struct PathPrefab : IComponentData
{
    public Entity prefab;
}

[InternalBufferCapacity(0)]
public struct UnitPath : IBufferElementData
{
    public int2 nodeIndex;
}

public struct PathRequestData : IComponentData
{
    public int2 target;
}