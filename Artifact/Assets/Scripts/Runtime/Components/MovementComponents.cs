using Unity.Entities;
using Unity.Mathematics;

public struct MoveDirection : IComponentData
{
    public AxialDirections value;

    public AxialDirections GetNextDirection(bool prevDirection = false)
    {
        return value.GetNextDirection(prevDirection);
    }
}

public struct Moving : IComponentData
{

}

public struct DestinationNode : IComponentData
{
    public float3 position;
    public int2 node;
}

public struct InDistance : IComponentData
{
    public bool value;
}

public struct MoveSpeed : IComponentData
{
    public float value;
}

public struct WaypointsMovement : IComponentData
{
    public int currentWaypointIndex;
}