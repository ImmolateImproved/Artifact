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
            .ForEach((ref DynamicBuffer<UnitPath> unitPath, in IndexInGrid gridPosition, in PathfindingTarget pathfindingTarget) =>
            {
                var findPathData = new PathfindingData(grid, neighbors);

                unitPath.Clear();
                if (gridPosition.value.Equals(pathfindingTarget.node))
                    return;

                var start = gridPosition.value;
                var end = pathfindingTarget.node;

                var path = unitPath.Reinterpret<int2>();

                findPathData.FindPath(start, end, path);

            }).Run();
    }

    public struct PathfindingData
    {
        //neighbors for hex tile (even rows)
        public readonly NativeArray<int2> neighbors;

        private readonly Grid grid;

        private NativeHashMap<int2, int> costSoFar;
        private NativeHashMap<int2, int2> pathTrack;

        private NativeMinHeap openSet;

        public PathfindingData(Grid grid, NativeArray<int2> neighbors)
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

        public void FindPath(int2 start, int2 end, DynamicBuffer<int2> path)
        {
            FindPath(start, end);
            ReconstructPath(start, end, path);
        }

        public int CalculatePathLength(int2 start, int2 end)
        {
            FindPath(start, end);

            var pathLength = 0;

            var node = end;
            while (!node.Equals(start))
            {
                pathLength++;
                node = pathTrack[node];
            }

            return pathLength;
        }

        private void FindPath(int2 start, int2 end)
        {
            if (start.Equals(end))
                return;

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
                    return;
                }

                for (int i = 0; i < neighbors.Length; i++)
                {
                    var neighborIndex = HexTileNeighbors.GetNeighbor(currentNode, neighbors[i]);

                    if (!grid.HasTile(neighborIndex))
                        continue;

                    if (!grid.IsWalkable(neighborIndex) && !neighborIndex.Equals(end))
                        continue;

                    var newCost = costSoFar[currentNode] + 1;
                    costSoFar.TryGetValue(neighborIndex, out var oldCost);

                    if (oldCost > 0 && newCost >= oldCost)
                        continue;

                    costSoFar[neighborIndex] = newCost;
                    pathTrack[neighborIndex] = currentNode;

                    var expectedCost = newCost + Grid.GetDistance(neighborIndex, end);

                    openSet.Push(new MinHeapNode(neighborIndex, expectedCost));
                }
            }
        }

        private void ReconstructPath(int2 start, int2 end, DynamicBuffer<int2> path)
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
    }
}