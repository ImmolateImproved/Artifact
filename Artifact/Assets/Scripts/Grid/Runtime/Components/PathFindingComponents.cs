using Unity.Entities;
using Unity.Mathematics;

[InternalBufferCapacity(0)]
public struct UnitPath : IBufferElementData
{
    public int2 nodeIndex;
}

public struct PathfindingTarget : IComponentData
{
    public int2 node;
    public bool pathNeeded;
}