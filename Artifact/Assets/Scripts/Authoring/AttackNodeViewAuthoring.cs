using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AttackNodeViewAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject attackPoiterPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new AttackNodeManager());

        dstManager.AddComponentData(entity, new AttackNodeView 
        {
            attackPointerPrefab = conversionSystem.GetPrimaryEntity(attackPoiterPrefab)
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(attackPoiterPrefab);
    }
}