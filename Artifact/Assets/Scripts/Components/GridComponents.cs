using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct GridConfiguration : IComponentData
{
    public int gridRadius;
    public float tileSlotRadius;
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

    private readonly float tileSlotRadius;

    public int NodeCount { get; private set; }

    public static readonly float2x2 NodeToPositionMatrix = new float2x2
    {
        c0 = new float2(math.sqrt(3), 0),
        c1 = new float2(math.sqrt(3) / 2, 3 / 2f)
    };

    public static readonly float2x2 PositionToNodeMatrix = math.inverse(NodeToPositionMatrix);

    public Grid(GridConfiguration gridConfig, Allocator allocator)
    {
        NodeCount = HexTileNeighbors.CalculateTilesCount(gridConfig.gridRadius);

        tileSlotRadius = gridConfig.tileSlotRadius;

        nodePositions = new NativeHashMap<int2, float2>(NodeCount, allocator);
        units = new NativeHashMap<int2, Entity>(NodeCount, allocator);
        nodeToTile = new NativeHashMap<int2, Entity>(NodeCount, allocator);
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

    public int2 PositionToNode(float3 position)
    {
        var fractionalNodeCoordinate = math.mul(PositionToNodeMatrix, new float2(position.x, position.z)) / tileSlotRadius;

        return AxialRound(fractionalNodeCoordinate);

        int2 AxialRound(float2 position)
        {
            var x = position.x;
            var y = position.y;

            var xgrid = Mathf.RoundToInt(x);
            var ygrid = Mathf.RoundToInt(y);
            x -= xgrid;
            y -= ygrid;

            if (math.abs(x) >= math.abs(y))
            {
                return new int2(xgrid + Mathf.RoundToInt(x + 0.5f * y), ygrid);
            }

            return new int2(xgrid, ygrid + Mathf.RoundToInt(y + 0.5f * x));
        }
    }

    public int2 AxialToOddr(int2 node)
    {
        var col = node.x + (node.y - (node.y & 1)) / 2;
        var row = node.y;

        return new int2(col, row);
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