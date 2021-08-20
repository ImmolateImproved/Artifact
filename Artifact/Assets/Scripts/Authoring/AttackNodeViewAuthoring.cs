using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AttackNodeViewAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject attackTilePrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AttackNodeManager());

        dstManager.AddComponentData(entity, new AttackNodeView 
        {
            attackTilePrefab = conversionSystem.GetPrimaryEntity(attackTilePrefab)
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(attackTilePrefab);
    }
}