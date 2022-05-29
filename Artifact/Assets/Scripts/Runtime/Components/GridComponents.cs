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
    public float tileSize;

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

    public static int2 PositionToNode(float3 position, float tileSlotRadius)
    {
        var fractionalNodeCoordinate = math.mul(PositionToNodeMatrix, new float2(position.x, position.z)) / tileSlotRadius;

        return AxialRound(fractionalNodeCoordinate);
    }

    public float3 NodeToPosition(int2 node)
    {
        return NodeToPosition(node, tileSlotRadius);
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
    public int2 current;
    public int2 previous;
}

public struct GridInitializedTag : IComponentData
{

}

public struct TileTag : IComponentData
{

}

public struct PathTile : IComponentData
{

}