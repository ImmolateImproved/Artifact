using Unity.Entities;
using Unity.Mathematics;

public struct Moving : IComponentData
{
    
}

public struct MoveSpeed : IComponentData
{
    public float value;
}

public struct WaypointsMovement : IComponentData
{
    public int currentWaypointIndex;
}