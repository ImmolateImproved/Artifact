using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class TilePrefabsAuthoring : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject tilePrefab;
    public GameObject rangeTilePrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GridSpawner { prefab = conversionSystem.GetPrimaryEntity(tilePrefab) });
        dstManager.AddComponentData(entity, new RangeTilePrefabRef { prefab = conversionSystem.GetPrimaryEntity(rangeTilePrefab) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(tilePrefab);
        referencedPrefabs.Add(rangeTilePrefab);
    }
}