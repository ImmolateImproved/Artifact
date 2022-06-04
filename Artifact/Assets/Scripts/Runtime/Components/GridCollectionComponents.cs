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

    private NativeHashMap<int2, Entity> objects;

    public readonly NativeArray<int2> neighbors;

    public int NodeCount { get; private set; }

    public Grid(int nodeCount)
    {
        NodeCount = nodeCount;

        nodePositions = new NativeHashMap<int2, float2>(NodeCount, Allocator.Persistent);
        objects = new NativeHashMap<int2, Entity>(NodeCount, Allocator.Persistent);


        neighbors = new NativeArray<int2>(HexTileNeighbors.Neighbors, Allocator.Persistent);
    }

    public JobHandle Dispose(JobHandle inputDeps)
    {
        var disposeDependencies = new NativeArray<JobHandle>(4, Allocator.Temp)
        {
            [0] = objects.Dispose(inputDeps),
            [1] = nodePositions.Dispose(inputDeps),
            [2] = neighbors.Dispose(inputDeps)
        };

        return JobHandle.CombineDependencies(disposeDependencies);
    }

    public Type AssociatedComponentType => typeof(GridTag);

    public NativeHashMap<int2, float2>.Enumerator GetAllNodePositions()
    {
        return nodePositions.GetEnumerator();
    }

    public float3 GetNodePosition(int2 nodeIndex)
    {
        var nodePos = nodePositions[nodeIndex];

        var position = new float3(nodePos.x, 0, nodePos.y);

        return position;
    }

    public void SetNodePosition(int2 index, float2 position)
    {
        nodePositions[index] = position;
    }

    public int2 GetNeighborNodeFromDirection(int2 currentNode, HexDirections direction)
    {
        var dir = neighbors[(int)direction];

        var nextNode = currentNode + dir;

        return nextNode;
    }

    public void SetGridObject(int2 nodeIndex, Entity gridObject)
    {
        objects.Add(nodeIndex, gridObject);
    }

    public Entity GetGridObject(int2 nodeIndex)
    {
        objects.TryGetValue(nodeIndex, out var gridObject);

        return gridObject;
    }

    public void RemoveGridObject(int2 nodeIndex)
    {
        objects.Remove(nodeIndex);
    }

    public bool HasGridOject(int2 nodeIndex)
    {
        var hasObject = objects.ContainsKey(nodeIndex);
        return hasObject;
    }

    public bool HasNode(int2 nodeIndex)
    {
        return nodePositions.TryGetValue(nodeIndex, out var _); ;
    }

    public bool IsWalkable(int2 nodeIndex)
    {
        return HasNode(nodeIndex) && !HasGridOject(nodeIndex);
    }

    public NativeList<int2> GetNeighborNodes(int2 startNode)
    {
        var neighbors = new NativeList<int2>(6, Allocator.Temp);

        var direction = HexDirections.BottomLeft;

        for (int i = 0; i < HexDirectionsExtentions.DIRECTIONS_COUNT; i++)
        {
            var nextNode = GetNeighborNodeFromDirection(startNode, direction);
            direction = direction.GetNextDirection();

            if (HasNode(nextNode))
            {
                neighbors.Add(nextNode);
            }
        }

        return neighbors;
    }

    public NativeList<int2> GetNeighborNodesInRange(int2 startNode, int findRange)
    {
        var neighborsInRange = new NativeList<int2>(6, Allocator.Temp);

        var nodesInRange = HexTileNeighbors.CalculateTilesCount(findRange);

        var queue = new NativeQueue<int2>(Allocator.Temp);
        var visited = new NativeHashSet<int2>(nodesInRange, Allocator.Temp);

        queue.Enqueue(startNode);
        visited.Add(startNode);

        while (visited.Count() < nodesInRange)
        {
            var currentNode = queue.Dequeue();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeighborNode(currentNode, neighbors[i]);

                if (!visited.Add(neighborNode))
                {
                    continue;
                }

                queue.Enqueue(neighborNode);

                neighborsInRange.Add(neighborNode);
            }
        }

        visited.Dispose();
        queue.Dispose();

        return neighborsInRange;
    }

    public NativeList<int2> FindGridObjects(int2 startNode, int findRange)
    {
        var neighborsInRange = GetNeighborNodesInRange(startNode, findRange);

        var gridObjectsInRange = new NativeList<int2>(6, Allocator.Temp);

        foreach (var node in neighborsInRange)
        {
            if (HasGridOject(node))
            {
                gridObjectsInRange.Add(node);
            }
        }

        return gridObjectsInRange;
    }
}

public struct TileGridData : ICollectionComponent
{
    private NativeHashMap<int2, Entity> nodeToTile;

    public TileGridData(int nodeCount)
    {
        nodeToTile = new NativeHashMap<int2, Entity>(nodeCount, Allocator.Persistent);
    }

    public Type AssociatedComponentType => typeof(GridTag);

    public JobHandle Dispose(JobHandle inputDeps)
    {
        return nodeToTile.Dispose(inputDeps);
    }

    public void InitTile(int2 nodeIndex, Entity tile)
    {
        nodeToTile.Add(nodeIndex, tile);
    }

    public Entity GetTile(int2 nodeIndex)
    {
        return nodeToTile.TryGetValue(nodeIndex, out var tile) ? tile : Entity.Null;
    }
}