using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

public enum AxialDirections
{
    Right, BottomRight, BottomLeft, Left, TopLeft, TopRight
}

public static class AxialDirectionsExtentions
{
    public const int DIRECTIONS_COUNT = 6;

    public static AxialDirections GetNextDirection(AxialDirections direction)
    {
        var intDirection = (int)direction;
        intDirection = (intDirection + 1) % DIRECTIONS_COUNT;

        return (AxialDirections)intDirection;
    }

    public static AxialDirections ReverseDirection(this AxialDirections direction)
    {
        var newDirection = ((int)direction + DIRECTIONS_COUNT / 2) % DIRECTIONS_COUNT;

        return (AxialDirections)newDirection;
    }

    public static AxialDirections FromVector(int2 self, int2 target, NativeArray<int2> neighbors)
    {
        var dir = target - self;

        for (int i = 0; i < neighbors.Length; i++)
        {
            var direction = neighbors[i];

            if (direction.Equals(dir))
            {
                return (AxialDirections)i;
            }
        }

        return default;
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

    public static int2 GetNeighbor(int2 current, int2 direction)
    {
        return current + direction;
    }

    public static bool IsNeightbors(this NativeArray<int2> array, int2 nodeA, int2 nodeB)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (GetNeighbor(nodeA, array[i]).Equals(nodeB))
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