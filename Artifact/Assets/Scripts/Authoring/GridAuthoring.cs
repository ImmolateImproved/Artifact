using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class GridAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public int width;
    public int height;

    public float tileRaius = 1;
    public float tileHeight;
    public float tileOffset;

    public TileAuthoring tilePrefab;
    public GameObject moveRangePrefab;
    public GameObject pathPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var moveRangeTileScale = tileRaius * 0.8f;
        moveRangePrefab.transform.localScale = new Vector3(moveRangeTileScale, 1, moveRangeTileScale);
        pathPrefab.transform.localScale = new Vector3(moveRangeTileScale, 1, moveRangeTileScale);

        dstManager.AddComponentData(entity, new GridConfiguration
        {
            height = height,
            width = width,
            tileScale = tileRaius,
            tilePrefab = conversionSystem.GetPrimaryEntity(tilePrefab)
        });

        dstManager.AddComponentData(entity, new PathPrefab { prefab = conversionSystem.GetPrimaryEntity(pathPrefab) });
        dstManager.AddComponentData(entity, new MoveRangePrefab { prefab = conversionSystem.GetPrimaryEntity(moveRangePrefab) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(tilePrefab.gameObject);
        referencedPrefabs.Add(pathPrefab);
        referencedPrefabs.Add(moveRangePrefab);
    }

    public void BuildGrid()
    {
        var mapHolder = CreateMapHolder();

        var tilePosY = -tileHeight;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var tilePos = GetPositionCellFromCoordinate(new Vector2Int(x, y));
                tilePos.y = tilePosY;

                var tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, mapHolder);

                tile.transform.localScale = new Vector3(tileRaius, tileHeight, tileRaius);

                tile.indexInGrid = new int2(x, y);
            }
        }
    }

    private Transform CreateMapHolder()
    {
        var holderName = "GeneratedMap";

        var mapHolder = GameObject.Find(holderName);

        if (mapHolder)
        {
            DestroyImmediate(mapHolder);
        }

        mapHolder = new GameObject(holderName);
        mapHolder.AddComponent<ConvertToEntity>();

        return mapHolder.transform;
    }

    private Vector3 GetPositionCellFromCoordinate(Vector2Int coordinate)
    {
        var column = coordinate.x;
        var row = coordinate.y;

        var tileSize = tileOffset + tileRaius;

        var width = Mathf.Sqrt(3) * tileSize;
        var height = 2f * tileSize;

        var horizontalDistance = width;
        var verticalDistance = height * (3 / 4f);

        var shouldOffset = (row % 2) == 1;
        var offset = shouldOffset ? width / 2 : 0;

        var xPosition = (column * horizontalDistance) + offset;
        var yPosition = row * verticalDistance;

        return new Vector3(xPosition, 0, -yPosition);
    }
}