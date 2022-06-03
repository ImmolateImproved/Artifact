using Unity.Entities;
using UnityEngine;

public class SettingAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int initialUnitEnergy;
    public int unitsDigestibility;
    public int maxEnergy;

    public int visionRange;

    public int moveEnergyPrice;
    public int attackEnergyPrice;
    public int energyForReproduction;

    public Color deathColor;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Settings
        {
            initialEnergy = initialUnitEnergy,
            maxEnergy = maxEnergy,
            digestibility = unitsDigestibility,

            visionRange = visionRange,

            moveEnergyPrice = moveEnergyPrice,
            attackEnergyPrice = attackEnergyPrice,
            energyForReproduction = energyForReproduction,

            deathColor = deathColor
        });
    }
}