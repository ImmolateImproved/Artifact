using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using Unity.Collections;

[DisallowMultipleComponent]
public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnitTypes unitType;
    public bool selectable;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<UnitTag>(entity);
        dstManager.AddComponentData(entity, new UnitType { value = unitType });
        dstManager.AddComponentData(entity, new AliveStatus { isAlive = true });

        var genome = new FixedList32Bytes<GenomeData>
        {
            new GenomeData { gene = Genes.Predation, weight = 1 },
            new GenomeData { gene = Genes.Herbivory, weight = 1 },
            new GenomeData { gene = Genes.Scavenger, weight = 1 }
        };

        dstManager.AddComponentData(entity, new Genome { value = genome });

        dstManager.AddComponent<IndexInGrid>(entity);

        dstManager.AddComponent<Energy>(entity);

        dstManager.AddBuffer<NeighborNode>(entity);

        if (selectable)
        {
            dstManager.AddComponent<Selectable>(entity);
        }
    }
}