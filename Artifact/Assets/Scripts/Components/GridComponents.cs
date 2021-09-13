using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

public struct GridConfiguration : IComponentData
{
    public int rows;
    public int columns;

    public float tileScale;

    public Entity tilePrefab;
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
    private NativeArray<float2> nodePositions;

    private NativeArray<Entity> units;

    [NativeDisableParallelForRestriction]
    private NativeHashMap<int2, Entity> nodeToTile;

    public readonly int columns;
    public readonly int rows;

    public Grid(int columns, int rows, Allocator allocator)
    {
        this.columns = columns;
        this.rows = rows;
        nodePositions = new NativeArray<float2>(rows * columns, allocator);
        units = new NativeArray<Entity>(rows * columns, allocator);
        nodeToTile = new NativeHashMap<int2, Entity>(rows * columns, allocator);
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        return JobHandle.CombineDependencies(units.Dispose(inputDeps), nodePositions.Dispose(inputDeps), nodeToTile.Dispose(inputDeps));
    }

    public Type AssociatedComponentType => typeof(GridTag);

    public float2 this[int x, int y]
    {
        get => nodePositions[y * columns + x];
        set => nodePositions[y * columns + x] = value;
    }

    public float2 this[int2 index]
    {
        get => this[index.x, index.y];
        set => this[index.x, index.y] = value;
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
        units[From2DIndex(index)] = unit;
    }

    public void RemoveUnit(int2 index)
    {
        units[From2DIndex(index)] = Entity.Null;
    }

    public Entity GetUnit(int2 index)
    {
        return units[From2DIndex(index)];
    }

    public bool HasUnit(int2 index)
    {
        return GetUnit(index) != Entity.Null;
    }

    public bool IsWalkable(int2 index)
    {
        return IsWalkable(index.x, index.y);
    }

    public bool IsWalkable(int x, int y)
    {
        return GetUnit(new int2(x, y)) == Entity.Null;
    }

    public int From2DIndex(int2 index)
    {
        return From2DIndex(index.x, index.y);
    }

    public int From2DIndex(int x, int y)
    {
        return y * columns + x;
    }

    public bool IndexInRange(int2 index)
    {
        return index.x >= 0 && index.x < columns && index.y >= 0 && index.y < rows;
    }
}