using Latios;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum HexDirections
{
    Right, BottomRight, BottomLeft, Left, TopLeft, TopRight
}

public static class HexDirectionsExtentions
{
    public const int DIRECTIONS_COUNT = 6;

    public static HexDirections GetNextDirection(this HexDirections direction, bool prevDirection = false)
    {
        var intDirection = (int)direction;

        intDirection = prevDirection ? intDirection - 1 : intDirection + 1;

        intDirection = intDirection > 0
                        ? intDirection % DIRECTIONS_COUNT
                        : intDirection + DIRECTIONS_COUNT;

        return (HexDirections)intDirection;
    }

    public static HexDirections GetReverseDirection(this HexDirections direction)
    {
        var newDirection = ((int)direction + DIRECTIONS_COUNT / 2) % DIRECTIONS_COUNT;

        return (HexDirections)newDirection;
    }

    public static HexDirections GetRandomDirection(ref Rng.RngSequence rngSequence)
    {
        var randomIndex = rngSequence.NextInt(0, DIRECTIONS_COUNT);

        return (HexDirections)randomIndex;
    }
}

public static class HexTileNeighbors
{
    public static readonly int2[] Neighbors = new int2[]
    {
        new int2(1, 0),
        new int2(1, -1),
        new int2(0, -1),
        new int2(-1, 0),
        new int2(-1, 1),
        new int2(0, 1)
    };

    public static int2 GetNeighborNode(int2 current, int2 direction)
    {
        return current + direction;
    }

    public static bool IsNeighbors(this NativeArray<int2> array, int2 nodeA, int2 nodeB)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (GetNeighborNode(nodeA, array[i]).Equals(nodeB))
            {
                return true;
            }
        }

        return false;
    }

    public static int CalculateTilesCount(int radius)
    {
        var neighborCount = 6;

        var count = 1;

        for (int i = 1; i <= radius; i++)
        {
            count += neighborCount * i;
        }

        return count;
    }
}