using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class AttackNodeManagerAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject attackPoiterPrefab;

    public GridAuthoring gridAuthoring;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var scale = gridAuthoring.tileRaius * 0.8f;

        attackPoiterPrefab.transform.localScale = new Vector3(scale, scale, scale);

        dstManager.AddComponentData(entity, new TargetManager());

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