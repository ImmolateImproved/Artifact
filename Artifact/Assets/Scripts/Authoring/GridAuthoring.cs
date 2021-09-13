using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class GridAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public int width;
    public int height;

    public float tileScale;

    public GameObject tilePrefab;
    public GameObject pathPrefab;
    public GameObject moveRangePrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GridConfiguration
        {
            rows = height,
            columns = width,
            tileScale = tileScale,
            tilePrefab = conversionSystem.GetPrimaryEntity(tilePrefab)
        });

        dstManager.AddComponentData(entity, new PathPrefab { prefab = conversionSystem.GetPrimaryEntity(pathPrefab) });
        dstManager.AddComponentData(entity, new MoveRangePrefab { prefab = conversionSystem.GetPrimaryEntity(moveRangePrefab) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(tilePrefab);
        referencedPrefabs.Add(pathPrefab);
        referencedPrefabs.Add(moveRangePrefab);
    }

    public void BuildGrid()
    {
        var holderName = "GeneratedMap";

        var mapHolder = GameObject.Find(holderName).transform;

        if (mapHolder)
        {
            DestroyImmediate(mapHolder.gameObject);
        }

        mapHolder = new GameObject(holderName).transform;
        mapHolder.gameObject.AddComponent<ConvertToEntity>();

        for (int index = 0; index < width * height; index++)
        {
            float xOffset = 1.05f;
            float zOffset = 0.9f;

            int x = index % width;
            int y = index / width;

            float xPos = (x * xOffset * tileScale) - (width / 2);
            float yPos = (y * zOffset * tileScale) - (height / 2);

            if (y % 2 == 1)
            {
                xPos += xOffset * tileScale / 2;
            }

            var tile = Instantiate(tilePrefab, new Vector3(xPos, 0f, yPos), Quaternion.identity, mapHolder);
            tile.transform.localScale = new Vector3(tileScale, 1, tileScale);
            tile.GetComponent<TileAuthoring>().indexInGrid = new int2(x, y);
        }
    }

}