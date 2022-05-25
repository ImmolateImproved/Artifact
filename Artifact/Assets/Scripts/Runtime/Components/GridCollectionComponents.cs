using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public struct GridTag : IComponentData { }

public struct Grid : ICollectionComponent
{
    private NativeHashMap<int2, float2> nodePositions;

    private NativeMultiHashMap<int2, Entity> objects;

    private NativeHashMap<int2, Entity> nodeToTile;

    public readonly NativeArray<int2> neighbors;

    public int NodeCount { get; private set; }

    public Grid(GridConfig gridConfig)
    {
        NodeCount = HexTileNeighbors.CalculateTilesCount(gridConfig.gridRadius);

        nodePositions = new NativeHashMap<int2, float2>(NodeCount, Allocator.Persistent);
        objects = new NativeMultiHashMap<int2, Entity>(NodeCount, Allocator.Persistent);
        nodeToTile = new NativeHashMap<int2, Entity>(NodeCount, Allocator.Persistent);

        neighbors = new NativeArray<int2>(HexTileNeighbors.Neighbors, Allocator.Persistent);
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        var disposeDependencies = new NativeArray<JobHandle>(4, Allocator.Temp)
        {
            [0] = objects.Dispose(inputDeps),
            [1] = nodePositions.Dispose(inputDeps),
            [2] = nodeToTile.Dispose(inputDeps),
            [3] = neighbors.Dispose(inputDeps)
        };

        return JobHandle.CombineDependencies(disposeDependencies);
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

    public NativeHashMap<int2, float2>.Enumerator GetNodePositions()
    {
        return nodePositions.GetEnumerator();
    }

    public int2 GetNextNode(int2 currentNode, AxialDirections direction)
    {
        var dir = neighbors[(int)direction];

        var nextNode = currentNode + dir;

        return nextNode;
    }

    public void InitTile(int2 nodeIndex, Entity tile)
    {
        nodeToTile.Add(nodeIndex, tile);
    }

    public Entity GetTile(int2 nodeIndex)
    {
        return nodeToTile.TryGetValue(nodeIndex, out var tile) ? tile : Entity.Null;
    }

    public void SetGridObjects(int2 nodeIndex, Entity gridObject)
    {
        if (CompareObjects(nodeIndex, gridObject))
            return;

        objects.Add(nodeIndex, gridObject);
    }

    public NativeMultiHashMap<int2, Entity>.Enumerator GetGridObjects(int2 nodeIndex)
    {
        var iterator = objects.GetValuesForKey(nodeIndex);

        return iterator;
    }

    public void RemoveGridObject(int2 nodeIndex, Entity gridObject)
    {
        objects.Remove(nodeIndex, gridObject);
    }

    public bool CompareObjects(int2 objectANode, Entity ojbectB)
    {
        var objectsOnTile = GetGridObjects(objectANode);

        foreach (var item in objectsOnTile)
        {
            var objectsAreSame = item == ojbectB;

            if (objectsAreSame)
                return true;
        }

        return false;
    }

    public bool HasGridOject(int2 nodeIndex)
    {
        var count = objects.CountValuesForKey(nodeIndex);

        return count > 0;
    }

    public bool HasNode(int2 nodeIndex)
    {
        var cellExist = nodePositions.TryGetValue(nodeIndex, out var _);

        return cellExist;
    }

    public bool IsWalkable(int2 nodeIndex)
    {
        return HasNode(nodeIndex) && !HasGridOject(nodeIndex);
    }

    public static int GetDistance(int2 nodeA, int2 nodeB)
    {
        var result = (math.abs(nodeA.x - nodeB.x) + math.abs(nodeA.x + nodeA.y - nodeB.x - nodeB.y) + math.abs(nodeA.y - nodeB.y)) / 2;

        return result;
    }

    public void FindGridObjects(int2 start, int findRange, NativeList<Entity> gridObjectsInRange)
    {
        var nodesInRange = HexTileNeighbors.CalculateTilesCount(findRange);

        var queue = new NativeQueue<int2>(Allocator.Temp);
        var visited = new NativeHashSet<int2>(nodesInRange, Allocator.Temp);

        queue.Enqueue(start);
        visited.Add(start);

        while (visited.Count() <= nodesInRange)
        {
            var currentNode = queue.Dequeue();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeighbor(currentNode, neighbors[i]);

                if (!visited.Add(neighborNode))
                {
                    continue;
                }

                queue.Enqueue(neighborNode);

                if (!HasGridOject(neighborNode))
                    continue;

                var gridObjects = GetGridObjects(neighborNode);

                foreach (var item in gridObjects)
                {
                    gridObjectsInRange.Add(item);
                }
            }
        }

        visited.Dispose();
        queue.Dispose();
    }
}