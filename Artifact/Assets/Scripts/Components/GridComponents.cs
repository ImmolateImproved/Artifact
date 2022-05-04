using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct GridConfiguration : IComponentData
{
    public int height;
    public int width;
}

public struct IndexInGrid : IComponentData
{
    public int2 value;
}

public struct PreviousGridIndex : IComponentData
{
    public int2 value;
}

public struct GridTag : IComponentData { }

public struct Grid : ICollectionComponent
{
    private NativeHashMap<int2, float2> nodePositions;

    private NativeHashMap<int2, Entity> units;

    private NativeHashMap<int2, Entity> nodeToTile;

    public readonly int width;
    public readonly int height;

    public Grid(GridConfiguration gridConfig, Allocator allocator)
    {
        width = gridConfig.width;
        height = gridConfig.height;

        nodePositions = new NativeHashMap<int2, float2>(height * width, allocator);
        units = new NativeHashMap<int2, Entity>(height * width, allocator);
        nodeToTile = new NativeHashMap<int2, Entity>(height * width, allocator);
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        return JobHandle.CombineDependencies(units.Dispose(inputDeps), nodePositions.Dispose(inputDeps), nodeToTile.Dispose(inputDeps));
    }

    public Type AssociatedComponentType => typeof(GridTag);

    public float2? this[int x, int y]
    {
        get
        {
            var index = new int2(x, y);
            return this[index];
        }
        set
        {
            var index = new int2(x, y);
            this[index] = value;
        }
    }

    public float2? this[int2 index]
    {
        get
        {
            if (nodePositions.TryGetValue(index, out var position))
            {
                return position;
            }

            return null;
        }

        set
        {
            nodePositions[index] = value.Value;
        }
    }

    public void InitTile(int2 nodeIndex, Entity tile)
    {
        nodeToTile.Add(nodeIndex, tile);
    }

    public Entity GetTile(int2 index)
    {
        return nodeToTile.TryGetValue(index, out var tile) ? tile : Entity.Null;
    }

    public void SetUnit(int2 index, Entity unit)
    {
        units[index] = unit;
    }

    public Entity GetUnit(int2 index)
    {
        units.TryGetValue(index, out var unit);

        return IndexInRange(index) ? unit : Entity.Null;
    }

    public bool HasUnit(int2 index)
    {
        return IndexInRange(index) && (GetUnit(index) != Entity.Null);
    }

    public void RemoveUnit(int2 index)
    {
        units[index] = Entity.Null;
    }

    public bool IsWalkable(int2 index)
    {
        return GetUnit(index) == Entity.Null;
    }

    public bool IsWalkable(int x, int y)
    {
        return IsWalkable(new int2(x, y));
    }

    public bool IndexInRange(int2 index)
    {
        var cellExist = nodePositions.TryGetValue(index, out var _);

        return cellExist;
    }
}