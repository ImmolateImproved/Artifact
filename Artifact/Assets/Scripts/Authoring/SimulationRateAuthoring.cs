using Unity.Entities;
using UnityEngine;

public class SimulationRateAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int simulationRate;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SimulationRate { value = simulationRate });
    }
}