using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class SelectDestinationSystem : SubSystem
{
    private Rng m_rng;

    public override void OnNewScene()
    {
        m_rng = new Rng("SelectDestinationSystem");
    }

    protected override void OnUpdate()
    {
        var rng = m_rng.Shuffle();

        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        Entities.ForEach((Entity e, int entityInQueryIndex, ref Destination destination, in Genome genome, in DynamicBuffer<NeighborNode> neighborsBuffer, in IndexInGrid indexInGrid, in AliveStatus aliveStatus) =>
        {
            if (!aliveStatus.isAlive)
                return;

            var neighbors = neighborsBuffer.AsNativeArray().Reinterpret<int2>();

            var potentialTargets = new NativeList<TargetData>(6, Allocator.Temp);

            var targetNode = destination.node;

            for (int i = 0; i < neighbors.Length; i++)
            {
                var node = neighbors[i];

                var unit = grid.GetGridObject(node);

                if (unit == Entity.Null)
                    continue;

                var unitType = GetComponent<UnitType>(unit).value;

                var genomeData = genome.GetGenomeDataFromUnitType(unitType);

                var targetData = new TargetData
                {
                    genomeData = genomeData,
                    node = node
                };

                potentialTargets.Add(targetData);
            }

            //if (potentialTargets.Length == 0)
            {
                var random = rng.GetSequence(entityInQueryIndex);
                targetNode = neighbors[random.NextInt(0, neighbors.Length)];
            }
            //else
            //{
            //    var result = Roll(potentialTargets);

            //    targetNode = result.node;
            //}
            if (grid.IsWalkable(targetNode))
            {
                destination.Set(targetNode, grid);

                grid.RemoveGridObject(indexInGrid.current);
                grid.SetGridObject(targetNode, e);
            }
            else
            {
                destination.Set(indexInGrid.current, grid);
            }

        }).Run();
    }

    private static TargetData Roll(NativeList<TargetData> targetsData)
    {
        var sumOfWeights = 0;

        foreach (var targetData in targetsData)
        {
            sumOfWeights += targetData.genomeData.weight;
        }

        var randomValue = UnityEngine.Random.Range(0, sumOfWeights);

        return LookupValue(randomValue, targetsData);
    }

    private static TargetData LookupValue(int randomValue, NativeList<TargetData> targetsData)
    {
        var cumulativeWeight = 0;

        foreach (var targetData in targetsData)
        {
            cumulativeWeight += targetData.genomeData.weight;
            if (randomValue < cumulativeWeight)
            {
                return targetData;
            }
        }

        return default;
    }
}