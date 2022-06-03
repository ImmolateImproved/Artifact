using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public static class GridUtils
{
    public static int Distance(int2 nodeA, int2 nodeB)
    {
        var result = (math.abs(nodeA.x - nodeB.x) + math.abs(nodeA.x + nodeA.y - nodeB.x - nodeB.y) + math.abs(nodeA.y - nodeB.y)) / 2;

        return result;
    }
}

public struct GridConfig : IComponentData
{
    public int GridRadius { get; private set; }
    public float TileSlotRadius { get; private set; }
    public float TileSize { get; private set; }
    public int NodesCount { get; private set; }

    public static readonly float2x2 NodeToPositionMatrix = new float2x2
    {
        c0 = new float2(math.sqrt(3), 0),
        c1 = new float2(math.sqrt(3) / 2, 3 / 2f)
    };

    public static readonly float2x2 PositionToNodeMatrix = math.inverse(NodeToPositionMatrix);

    public GridConfig(int gridRadius, float tileSize, float tileSlotRadius)
    {
        NodesCount = HexTileNeighbors.CalculateTilesCount(gridRadius);

        GridRadius = gridRadius;
        TileSlotRadius = tileSlotRadius;
        TileSize = tileSize;
    }

    public int2 PositionToNode(float3 position)
    {
        return PositionToNode(position, TileSlotRadius);
    }

    public static int2 PositionToNode(float3 position, float tileSlotRadius)
    {
        var fractionalNodeCoordinate = math.mul(PositionToNodeMatrix, new float2(position.x, position.z)) / tileSlotRadius;

        return AxialRound(fractionalNodeCoordinate);
    }

    public float3 NodeToPosition(int2 node)
    {
        return NodeToPosition(node, TileSlotRadius);
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

public struct GridSpawner : IComponentData
{
    public Entity prefab;
}

public struct IndexInGrid : IComponentData
{
    public int2 current;
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