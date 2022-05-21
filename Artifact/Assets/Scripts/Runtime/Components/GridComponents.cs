using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct GridConfig : IComponentData
{
    public int gridRadius;
    public float tileSlotRadius;

    public static readonly float2x2 NodeToPositionMatrix = new float2x2
    {
        c0 = new float2(math.sqrt(3), 0),
        c1 = new float2(math.sqrt(3) / 2, 3 / 2f)
    };

    public static readonly float2x2 PositionToNodeMatrix = math.inverse(NodeToPositionMatrix);

    public int2 PositionToNode(float3 position)
    {
        return PositionToNode(position, tileSlotRadius);
    }

    public float3 NodeToPosition(int2 node)
    {
        return NodeToPosition(node, tileSlotRadius);
    }

    public static int2 PositionToNode(float3 position, float tileSlotRadius)
    {
        var fractionalNodeCoordinate = math.mul(PositionToNodeMatrix, new float2(position.x, position.z)) / tileSlotRadius;

        return AxialRound(fractionalNodeCoordinate);
    }

    public static float3 NodeToPosition(int2 node, float tileSlotRadius)
    {
        var position = tileSlotRadius * math.mul(NodeToPositionMatrix, node);

        return new float3(position.x, 0, position.y);
    }

    public static int2 AxialToOddr(int2 node)
    {
        var col = node.x + (node.y - (node.y & 1)) / 2;
        var row = node.y;

        return new int2(col, row);
    }

    public static int2 OddrToAxial(int2 node)
    {
        var q = node.x - (node.y - (node.y & 1)) / 2;
        var r = node.y;

        return new int2(q, r);
    }

    private static int2 AxialRound(float2 position)
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

    public int NodeCount { get; private set; }

    public Grid(GridConfig gridConfig, Allocator allocator)
    {
        NodeCount = HexTileNeighbors.CalculateTilesCount(gridConfig.gridRadius);

        nodePositions = new NativeHashMap<int2, float2>(NodeCount, allocator);
        units = new NativeHashMap<int2, Entity>(NodeCount, allocator);
        nodeToTile = new NativeHashMap<int2, Entity>(NodeCount, allocator);
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        return JobHandle.CombineDependencies(units.Dispose(inputDeps), nodePositions.Dispose(inputDeps), nodeToTile.Dispose(inputDeps));
    }

    public Type AssociatedComponentType => typeof(GridTag);

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

    public Entity GetTile(int2 nodeIndex)
    {
        return nodeToTile.TryGetValue(nodeIndex, out var tile) ? tile : Entity.Null;
    }

    public void SetUnit(int2 nodeIndex, Entity unit)
    {
        units[nodeIndex] = unit;
    }

    public Entity GetUnit(int2 nodeIndex)
    {
        var hasUnit = units.TryGetValue(nodeIndex, out var unit);

        return hasUnit ? unit : Entity.Null;
    }

    public bool HasUnit(int2 nodeIndex)
    {
        return GetUnit(nodeIndex) != Entity.Null;
    }

    public void RemoveUnit(int2 nodeIndex)
    {
        units[nodeIndex] = Entity.Null;
    }

    public bool IsWalkable(int2 nodeIndex)
    {
        return HasTile(nodeIndex) && !HasUnit(nodeIndex);
    }

    public bool HasTile(int2 nodeIndex)
    {
        var cellExist = nodePositions.TryGetValue(nodeIndex, out var _);

        return cellExist;
    }

    public static int GetDistance(int2 nodeA, int2 nodeB)
    {
        var result = (math.abs(nodeA.x - nodeB.x) + math.abs(nodeA.x + nodeA.y - nodeB.x - nodeB.y) + math.abs(nodeA.y - nodeB.y)) / 2;

        return result;
    }
}