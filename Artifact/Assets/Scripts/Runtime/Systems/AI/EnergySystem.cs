using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using UnityEngine;

public partial class EnergySystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();
        var settings = sceneBlackboardEntity.GetComponentData<Settings>();

        var sunEntity = GetSingletonEntity<Sun>();

        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities.ForEach((Entity e, ref Energy energy, ref UnitType unitType, ref URPMaterialPropertyBaseColor unitColor, in IndexInGrid indexInGrid) =>
        {
            energy.energy -= settings.moveEnergyPrice;

            var sun = GetComponent<Sun>(sunEntity);
            var sunIndex = GetComponent<IndexInGrid>(sunEntity);

            var distance = GridUtils.Distance(indexInGrid.current, sunIndex.current);

            if (distance <= sun.range)
            {
                energy.energy += energy.digestibility * (math.max(sun.range - distance, 1));
                energy.energy = math.min(energy.energy, energy.maxEnergy);
            }

            if (energy.energy <= 0)
            {
                unitColor.Value = (Vector4)settings.deathColor;
                unitType.value = UnitTypes.Coprse;

                ecb.RemoveComponent<DestinationNode>(e);
            }

        }).Run();

        ecb.Playback(EntityManager);
    }
}