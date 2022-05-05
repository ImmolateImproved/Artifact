using Unity.Collections;
using Unity.Mathematics;

public static class HexTileNeighbors
{
    public static readonly NativeArray<int2> Neighbors = new NativeArray<int2>(6, Allocator.Persistent)
    {
        [0] = new int2(1, 0),
        [1] = new int2(1, -1),
        [2] = new int2(0, -1),
        [3] = new int2(-1, 0),
        [4] = new int2(-1, 1),
        [5] = new int2(0, 1)
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