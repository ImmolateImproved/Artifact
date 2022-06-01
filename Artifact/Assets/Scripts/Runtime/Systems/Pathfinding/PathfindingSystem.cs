using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class PathfindingSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var pathfinder = new AStarPathfinding(grid);

        Entities.WithAll<Moving>()
          .ForEach((ref DestinationNode destinationNode, ref IndexInGrid indexInGrid, ref WaypointsMovement waypointsMovement,
          ref DynamicBuffer<UnitPath> path, ref PathfindingTarget pathfindingTarget, in InDistance inDistance) =>
                   {
                       if (!inDistance.value)
                           return;

                       if (pathfindingTarget.pathNeeded)
                       {
                           pathfinder.FindPath(indexInGrid.current, pathfindingTarget.node, path.Reinterpret<int2>());
                           waypointsMovement.currentWaypointIndex = 0;

                           pathfindingTarget.pathNeeded = false;
                       }

                       if (path.Length == 0)
                           return;

                       destinationNode.node = path[waypointsMovement.currentWaypointIndex].nodeIndex;
                       destinationNode.position = grid.GetNodePosition(destinationNode.node);

                       waypointsMovement.currentWaypointIndex++;

                       if (waypointsMovement.currentWaypointIndex == path.Length)
                       {
                           path.Length = 0;
                       }

                   }).Run();
    }

    public struct AStarPathfinding
    {
        [ReadOnly]
        private readonly NativeArray<int2> neighbors;

        [ReadOnly]
        private readonly Grid grid;

        private NativeHashMap<int2, int> costSoFar;
        private NativeHashMap<int2, int2> pathTrack;

        private NativeMinHeap openSet;

        public AStarPathfinding(Grid grid)
        {
            this = default;
            this.grid = grid;

            costSoFar = new NativeHashMap<int2, int>(64, Allocator.Temp);
            pathTrack = new NativeHashMap<int2, int2>(64, Allocator.Temp);

            openSet = new NativeMinHeap(grid.NodeCount * grid.NodeCount * 5, Allocator.Temp);

            neighbors = grid.neighbors;
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
                    var neighborIndex = HexTileNeighbors.GetNeighborNode(currentNode, neighbors[i]);

                    if (!grid.HasNode(neighborIndex) && !neighborIndex.Equals(end))
                        continue;

                    var newCost = costSoFar[currentNode] + 1;
                    costSoFar.TryGetValue(neighborIndex, out var oldCost);

                    if (oldCost > 0 && newCost >= oldCost)
                        continue;

                    costSoFar[neighborIndex] = newCost;
                    pathTrack[neighborIndex] = currentNode;

                    var expectedCost = newCost + GridUtils.Distance(neighborIndex, end);

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