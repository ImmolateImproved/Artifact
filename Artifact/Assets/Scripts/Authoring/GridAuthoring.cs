using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace Dragons
{
    [DisallowMultipleComponent]
    public class GridAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
    {
        public int gridWidth;

        public GameObject tilePrefab0;
        public GameObject pathPrefab;
        public GameObject moveRangePrefab;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new GridConfiguration
            {
                cellsPerRow = gridWidth,
                rows = gridWidth,
                tilePrefab = conversionSystem.GetPrimaryEntity(tilePrefab0)
            });

            dstManager.AddComponentData(entity, new PathPrefab { prefab = conversionSystem.GetPrimaryEntity(pathPrefab) });
            dstManager.AddComponentData(entity, new MoveRangePrefab { prefab = conversionSystem.GetPrimaryEntity(moveRangePrefab) });
        }

        public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(tilePrefab0);
            referencedPrefabs.Add(pathPrefab);
            referencedPrefabs.Add(moveRangePrefab);
        }
    }
}

