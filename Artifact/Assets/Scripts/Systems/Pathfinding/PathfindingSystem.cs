using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public partial class PathfindingSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var neighbors = HexTileNeighbors.Neighbors;

        Entities.WithAll<ActionRequest>()
            .ForEach((ref DynamicBuffer<UnitPath> pathBuffer, in IndexInGrid gridPosition, in PathfindingTarget pathfindingTarget) =>
            {
                var findPathData = new FindPathData(grid, neighbors);

                pathBuffer.Clear();
                if (gridPosition.value.Equals(pathfindingTarget.node))
                    return;

                var start = gridPosition.value;
                var end = pathfindingTarget.node;

                var path = pathBuffer.Reinterpret<int2>();

                findPathData.FindPath(start, end, path);

            }).Run();
    }

    public struct FindPathData
    {
        //neighbors for hex tile (even rows)
        public readonly NativeArray<int2> neighbors;

        private readonly Grid grid;

        private DynamicBuffer<int2> path;

        private NativeHashMap<int2, int> costSoFar;
        private NativeHashMap<int2, int2> pathTrack;

        private NativeMinHeap openSet;

        private int2 start;
        private int2 end;

        public FindPathData(Grid grid, NativeArray<int2> neighbors)
        {
            this = default;
            this.grid = grid;

            costSoFar = new NativeHashMap<int2, int>(64, Allocator.Temp);
            pathTrack = new NativeHashMap<int2, int2>(64, Allocator.Temp);

            openSet = new NativeMinHeap(grid.NodeCount * grid.NodeCount * 5, Allocator.Temp);

            this.neighbors = neighbors;
        }

        public void Dispose()
        {
            if (costSoFar.IsCreated)
            {
                costSoFar.Dispose();
                openSet.Dispose();
                pathTrack.Dispose();
            }
        }

        public void FindPath(in int2 start, in int2 end, in DynamicBuffer<int2> path)
        {
            if (start.Equals(end))
                return;

            this.start = start;
            this.end = end;
            this.path = path;

            costSoFar.Clear();
            pathTrack.Clear();
            openSet.Clear();

            openSet.Push(new MinHeapNode(start, 0));

            costSoFar[start] = 0;

            while (openSet.HasNext())
            {
                var currentIndex = openSet.Pop();
                var currentNode = openSet[currentIndex].Position;

                if (currentNode.Equals(end))
                {
                    ReconstructPath();
                    return;
                }

                for (int i = 0; i < neighbors.Length; i++)
                {
                    var neighborIndex = HexTileNeighbors.GetNeighbor(currentNode, neighbors[i]);

                    if (!grid.IndexInRange(neighborIndex))
                        continue;

                    if (!grid.IsWalkable(neighborIndex) && !neighborIndex.Equals(end))
                        continue;

                    var newCost = costSoFar[currentNode] + 1;
                    costSoFar.TryGetValue(neighborIndex, out var oldCost);

                    if (oldCost > 0 && newCost >= oldCost)
                        continue;

                    costSoFar[neighborIndex] = newCost;
                    pathTrack[neighborIndex] = currentNode;

                    var expectedCost = newCost + GetDistance(neighborIndex, end);

                    openSet.Push(new MinHeapNode(neighborIndex, expectedCost));
                }
            }
        }

        private void ReconstructPath()
        {
            var node = end;
            while (!node.Equals(start))
            {
                path.Add(node);
                node = pathTrack[node];
            }

            for (int i = 0; i < path.Length / 2; i++)
            {
                var tmp = path[i];
                path[i] = path[path.Length - i - 1];
                path[path.Length - i - 1] = tmp;
            }

            if (!grid.IsWalkable(end))
            {
                path.RemoveAt(path.Length - 1);
            }
        }

        private int GetDistance(int2 a, int2 b)
        {
            var result = (math.abs(a.x - b.x) + math.abs(a.x + a.y - b.x - b.y) + math.abs(a.y - b.y)) / 2;

            return result;
        }
    }
}