using Unity.Entities;
using Unity.Mathematics;

public struct MoveRangePrefab : IComponentData
{
    public Entity prefab;
}

public struct PathPrefab : IComponentData
{
    public Entity prefab;
}

[InternalBufferCapacity(0)]
public struct UnitPath : IBufferElementData
{
    public int2 nodeIndex;
}

public struct PathfindingTarget : IComponentData
{
    public int2 node;
}

public struct CalculateMoveRange : IComponentData
{

}

public struct DrawPath : IComponentData
{
    
}