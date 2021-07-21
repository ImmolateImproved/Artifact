using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
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

public struct MoveRange : IComponentData
{
    public int value;
}

public struct MoveRangeAssociated : IComponentData
{
    
}

public struct MoveRangeSet : ICollectionComponent
{
    public NativeHashSet<int2> moveRangeHashSet;

    public MoveRangeSet(int capacity, Allocator allocator)
    {
        moveRangeHashSet = new NativeHashSet<int2>(capacity, allocator);
    }

    public Type AssociatedComponentType => typeof(MoveRangeAssociated);

    public JobHandle Dispose(JobHandle inputDeps)
    {
        return moveRangeHashSet.Dispose(inputDeps);
    }
}