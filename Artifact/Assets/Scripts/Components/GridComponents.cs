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
    private NativeArray<float2> nodePositions;

    private NativeArray<Entity> units;

    private NativeHashMap<int2, Entity> nodeToTile;

    public readonly int width;
    public readonly int height;

    public Grid(GridConfiguration gridConfig, Allocator allocator)
    {
        width = gridConfig.width;
        height = gridConfig.height;

        nodePositions = new NativeArray<float2>(height * width, allocator);
        units = new NativeArray<Entity>(height * width, allocator);
        nodeToTile = new NativeHashMap<int2, Entity>(height * width, allocator);
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        return JobHandle.CombineDependencies(units.Dispose(inputDeps), nodePositions.Dispose(inputDeps), nodeToTile.Dispose(inputDeps));
    }

    public Type AssociatedComponentType => typeof(GridTag);

    public float2 this[int x, int y]
    {
        get => nodePositions[y * width + x];
        set => nodePositions[y * width + x] = value;
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

    public static Vector2 GetTileOffset(float tilesMargin, float tileRadius)
    {
        var tileSlotRadius = ((tilesMargin * tileRadius) + tileRadius);

        var xOffset = Mathf.Sqrt(3) * tileSlotRadius;
        var yOffset = 2f * tileSlotRadius;

        return new Vector2(xOffset, yOffset * (3 / 4f));
    }

    public static int2 PositionToGridIndex(Vector3 position, float tilesMargin, float tileRadius)
    {
        var tileOffset = GetTileOffset(tilesMargin, tileRadius);

        var xIndex = Mathf.FloorToInt(position.x / tileOffset.x);
        var yIndex = Mathf.RoundToInt(position.z / tileOffset.y);

        return new int2(xIndex, yIndex);
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
        return IndexInRange(index) ? units[From2DIndex(index)] : Entity.Null;
    }

    public bool HasUnit(int2 index)
    {
        return IndexInRange(index) && (GetUnit(index) != Entity.Null);
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
        return y * width + x;
    }

    public bool IndexInRange(int2 index)
    {
        return index.x >= 0 && index.x < width && index.y >= 0 && index.y < height;
    }
}