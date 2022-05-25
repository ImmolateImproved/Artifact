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

    public int notificationRange;

    public GridObjectTypes target;
}

public struct NotificationListener : IComponentData
{
    public int stepsToBase;
    public int stepsToResource;

    public int2 notifierNode;
    public bool changed;
}