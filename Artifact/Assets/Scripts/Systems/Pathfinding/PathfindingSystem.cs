using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class PathfindingSystem : SubSystem
{
    private FindPathData findPathData;

    protected override void OnStartRunning()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        findPathData.Dispose();
        findPathData = new FindPathData(grid);
    }

    protected override void OnStopRunning()
    {
        findPathData.Dispose();
    }

    protected override void OnDestroy()
    {
        findPathData.Dispose();
    }

    protected override void OnUpdate()
    {
        var findPathData = this.findPathData;

        Entities.WithAll<ExecutionRequest>()
            .ForEach((ref DynamicBuffer<UnitPath> pathBuffer, in ExecutionRequest executing, in IndexInGrid gridPosition, in PathRequestData pathRequest) =>
            {
                pathBuffer.Clear();

                var start = gridPosition.value;
                var end = pathRequest.target;

                var path = pathBuffer.Reinterpret<int2>();

                findPathData.FindPath(start, end, path);

            }).Run();
    }

    public struct FindPathData
    {
        //neighbors for hex tile (even rows)
        public static readonly int2[] neighbors =
        {
            new int2(-1, 0),
            new int2(-1, 1),
            new int2(0, 1),
            new int2(1, 0),
            new int2(0, -1),
            new int2(-1, -1)
        };

        private readonly Grid grid;

        private DynamicBuffer<int2> path;

        private NativeHashMap<int2, int> costs;
        private NativeHashMap<int2, int2> pathTrack;

        private NativeMinHeap openSet;

        private int2 start;
        private int2 end;

        public FindPathData(Grid grid)
        {
            this = default;
            this.grid = grid;

            costs = new NativeHashMap<int2, int>(64, Allocator.Persistent);
            pathTrack = new NativeHashMap<int2, int2>(64, Allocator.Persistent);

            openSet = new NativeMinHeap(grid.columns * grid.rows * 5, Allocator.Persistent);
        }

        public void Dispose()
        {
            if (costs.IsCreated)
            {
                costs.Dispose();
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

            costs.Clear();
            pathTrack.Clear();
            openSet.Clear();

            openSet.Push(new MinHeapNode(start, 0));

            costs[start] = 0;

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
                    var neighborIndex = GetNeightbor(currentNode, neighbors[i]);

                    if (!grid.IndexInRange(neighborIndex))
                        continue;

                    if (!grid.IsWalkable(neighborIndex) && !neighborIndex.Equals(end))
                        continue;

                    var newCost = costs[currentNode] + 1;
                    costs.TryGetValue(neighborIndex, out var oldCost);

                    if (oldCost > 0 && newCost >= oldCost)
                        continue;

                    costs[neighborIndex] = newCost;
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

        private int2 GetNeightbor(int2 currentNode, int2 neighborOffset)
        {
            //for hex grid, if we on odd row - change the sign of a neighborOffset
            neighborOffset = math.select(neighborOffset, -neighborOffset, currentNode.y % 2 == 1);

            return currentNode + neighborOffset;
        }

        private int GetDistance(int2 p0, int2 p1)
        {
            var dist = math.abs(p0 - p1);

            var multiplier = 2;

            return dist.x > dist.y
                   ? multiplier * dist.y + (dist.x - dist.y)
                   : multiplier * dist.x + (dist.y - dist.x);
        }
    }
}