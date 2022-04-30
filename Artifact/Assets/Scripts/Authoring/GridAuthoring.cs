using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class GridAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public int width;
    public int height;

    public float tileRadius = 1;
    public float tilesMargin;

    public TileAuthoring tilePrefab;
    public GameObject moveRangePrefab;
    public GameObject pathPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var moveRangeTileScale = tileRadius * 0.8f;
        moveRangePrefab.transform.localScale = new Vector3(moveRangeTileScale, 1, moveRangeTileScale);
        pathPrefab.transform.localScale = new Vector3(moveRangeTileScale, 1, moveRangeTileScale);

        dstManager.AddComponentData(entity, new GridConfiguration
        {
            height = height,
            width = width
        });

        dstManager.AddComponentData(entity, new PathPrefab { prefab = conversionSystem.GetPrimaryEntity(pathPrefab) });
        dstManager.AddComponentData(entity, new MoveRangePrefab { prefab = conversionSystem.GetPrimaryEntity(moveRangePrefab) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(pathPrefab);
        referencedPrefabs.Add(moveRangePrefab);
    }

    public void BuildGrid()
    {
        var mapHolder = CreateMapHolder();

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var tilePos = GetPositionCellFromCoordinate(new Vector2Int(x, y));
                tilePos.y -= 1;

                var tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, mapHolder);

                tile.transform.localScale = new Vector3(tileRadius, 1, tileRadius);

                tile.indexInGrid = new int2(x, y);
                tile.testIndices = Grid.PositionToGridIndex(tilePos, tilesMargin, tileRadius);
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

        var tileOffset = Grid.GetTileOffset(tilesMargin, tileRadius);

        var shouldOffset = (row % 2) == 1;
        var oddRowOffset = shouldOffset ? tileOffset.x / 2 : 0;

        var xPosition = (column * tileOffset.x) + oddRowOffset;
        var zPosition = row * tileOffset.y;

        var tilePos = new Vector3(xPosition, 0, zPosition);

        return tilePos;
    }
}