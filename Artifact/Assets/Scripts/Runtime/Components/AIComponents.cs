using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;

public struct Settings : IComponentData
{
    public int initialEnergy;
    public int maxEnergy;
    public int digestibility;

    public int visionRange;

    public int moveEnergyPrice;
    public int attackEnergyPrice;
    public int energyForReproduction;

    public Color deathColor;
}

public struct Sun : IComponentData
{
    public int range;
}

public struct Energy : IComponentData
{
    public int energy;
    public int maxEnergy;
    public int digestibility;
}

public struct AliveStatus : IComponentData
{
    public bool isAlive;
}

public enum Genes
{
    Predation, Herbivory, Scavenger
}

public enum UnitTypes
{
    LivingEntity, Grass, Coprse
}

public struct UnitType : IComponentData
{
    public UnitTypes value;
}

public struct GenomeData
{
    public Genes gene;
    public int weight;
}

public struct Genome : IComponentData
{
    public FixedList32Bytes<GenomeData> value;

    public GenomeData GetGenomeDataFromUnitType(UnitTypes unitType)
    {
        foreach (var genomeData in value)
        {
            if (genomeData.gene == (Genes)unitType)
            {
                return genomeData;
            }
        }

        return default;
    }
}

public struct TargetData
{
    public int2 node;
    public GenomeData genomeData;
}

public struct NeighborNode : IBufferElementData
{
    public int2 value;
}