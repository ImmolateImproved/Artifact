using Unity.Entities;
using Unity.Mathematics;

public struct MoveDirection : IComponentData
{
    public AxialDirections value;
}

public struct SwarmIntelligenceData : IComponentData
{
    public int stepsToBase;
    public int stepsToResource;

    public int stepsCounter;

    public GridObjectTypes target;
}

public struct NotificationRange : IComponentData
{
    public int value;
}

public struct NeighborGridObjects : IBufferElementData
{
    public int2 node;
    public GridObjectTypes objectType;
}

public struct NotificationListener : IComponentData
{
    public int stepsToBase;
    public int stepsToResource;

    public int2 notifierNode;
    public bool changed;
}