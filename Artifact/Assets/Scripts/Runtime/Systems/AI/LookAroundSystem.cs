using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class LookAroundSystem : SubSystem
{
    private Rng m_rng;

    public override void OnNewScene()
    {
        m_rng = new Rng("LookAroundSystem");
    }

    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var rng = m_rng.Shuffle();

        Entities.ForEach((int entityInQueryIndex, ref DestinationNode destination, ref MoveDirection moveDirection, in Genome genome, in IndexInGrid indexInGrid, in AliveStatus aliveStatus) =>
        {
            if (!aliveStatus.isAlive)
                return;

            var random = rng.GetSequence(entityInQueryIndex);

            var neighbors = new NativeList<int2>(6, Allocator.Temp);

            for (int i = 0; i < HexDirectionsExtentions.DIRECTIONS_COUNT; i++)
            {
                moveDirection.value = HexDirectionsExtentions.GetRandomDirection(ref random);

                var nextNode = grid.GetNeighborNodeFromDirection(indexInGrid.current, moveDirection.value);

                if (grid.IsWalkable(nextNode))
                {
                    neighbors.Add(nextNode);
                }
            }

            var potentialTargets = new NativeList<TargetData>(6, Allocator.Temp);

            for (int i = 0; i < neighbors.Length; i++)
            {
                var node = neighbors[i];

                if (grid.HasGridOject(node))
                {
                    var unit = grid.GetGridObject(node);

                    var unitType = GetComponent<UnitType>(unit).value;

                    var genomeData = genome.GetGenomeDataFromUnitType(unitType);

                    var targetData = new TargetData
                    {
                        genomeData = genomeData,
                        node = node
                    };

                    potentialTargets.Add(targetData);
                }
            }

            var result = Roll(potentialTargets);

        }).Run();
    }

    public static TargetData Roll(NativeList<TargetData> targetsData)
    {
        var sumOfWeights = 0;

        foreach (var targetData in targetsData)
        {
            sumOfWeights += targetData.genomeData.weight;
        }

        var randomValue = UnityEngine.Random.Range(0, sumOfWeights);

        return LookupValue(randomValue, targetsData);
    }

    public static TargetData LookupValue(int randomValue, NativeList<TargetData> targetsData)
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