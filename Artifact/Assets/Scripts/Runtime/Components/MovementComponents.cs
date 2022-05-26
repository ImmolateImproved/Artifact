using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

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