using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public struct MoveSpeed : IComponentData
{
    public float value;
}

public struct WaypointsMovement : IComponentData
{
    public int currentWaypointIndex;
}

public struct MoveTarget : IComponentData
{
    public int2 node;
    public float3 position;

    public void Set(int2 node, in Grid grid)
    {
        this.node = node;
        position = grid.GetNodePosition(node);
    }
}

public struct MovementRange : IComponentData
{
    public int value;
}