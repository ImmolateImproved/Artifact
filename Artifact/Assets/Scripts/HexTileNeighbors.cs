using Unity.Collections;
using Unity.Mathematics;

public static class HexTileNeighbors
{
    public static readonly NativeArray<int2> Neighbors = new NativeArray<int2>(6, Allocator.Persistent)
    {
        [0] = new int2(-1, 0),
        [1] = new int2(-1, 1),
        [2] = new int2(0, 1),
        [3] = new int2(1, 0),
        [4] = new int2(0, -1),
        [5] = new int2(-1, -1)
    };

    public static int2 GetNeightbor(int2 currentNode, int2 neighborOffset)
    {
        //for hex grid, if we on odd row - change the sign of a neighborOffset
        neighborOffset = math.select(neighborOffset, -neighborOffset, currentNode.y % 2 == 1);

        return currentNode + neighborOffset;
    }

    public static int CalculateTilesCount(int moveRange, int neighborCount)
    {
        var count = moveRange == 0 ? 0 : 1;

        for (int i = 0; i < moveRange; i++)
        {
            count += neighborCount * i;
        }

        return count;
    }
}