using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AIManagerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject[] aiUnits;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var buffer = dstManager.AddBuffer<AIUnits>(entity).Reinterpret<Entity>();

        for (int i = 0; i < aiUnits.Length; i++)
        {
            buffer.Add(conversionSystem.GetPrimaryEntity(aiUnits[i]));
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(aiUnits);
    }
}