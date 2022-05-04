using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public enum GridType
{
    Square, Circle
}

[DisallowMultipleComponent]
public class GridAuthoring : MonoBehaviour, IDeclareReferencedPrefabs, IConvertGameObjectToEntity
{
    public GridType gridType;

    [Header("For square grid")]
    public int width;
    public int height;

    [Header("For circle grid")]
    public int gridRadius;

    [Space]
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

    public void GenerateGrid()
    {
        switch (gridType)
        {
            case GridType.Square:
                {
                    GenerateSquareGrid();
                    break;
                }

            case GridType.Circle:
                {
                    GenerateCircleGrid();
                    break;
                }
        }
    }

    private void GenerateSquareGrid()
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
                tile.testIndices = PositionToGridIndex(tilePos);
            }
        }
    }

    private void GenerateCircleGrid()
    {
        var mapHolder = CreateMapHolder();

        var nodes = BuidGridBFS();

        foreach (var node in nodes)
        {
            var vectorNode = new Vector2Int(node.x, node.y);

            var tilePos = GetPositionCellFromCoordinate(vectorNode);
            tilePos.y -= 1;

            var tile = Instantiate(tilePrefab, tilePos, Quaternion.identity, mapHolder);

            tile.transform.localScale = new Vector3(tileRadius, 1, tileRadius);

            tile.indexInGrid = new int2(node.x, node.y);
            tile.testIndices = PositionToGridIndex(tilePos);
        }
    }

    private HashSet<int2> BuidGridBFS()
    {
        var neighbors = HexTileNeighbors.Neighbors;

        var tileInGridRadius = HexTileNeighbors.CalculateTilesCount(gridRadius);

        var queue = new Queue<int2>(tileInGridRadius);
        var visited = new HashSet<int2>();

        queue.Enqueue(new int2(gridRadius, gridRadius));

        while (visited.Count <= tileInGridRadius)
        {
            var node = queue.Dequeue();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeighbor(node, neighbors[i]);

                if (visited.Add(neighborNode))
                {
                    queue.Enqueue(neighborNode);
                }
            }
        }

        return visited;
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

        var tileOffset = GetTileOffset();

        var shouldOffset = (row % 2) == 1;
        var oddRowOffset = shouldOffset ? tileOffset.x / 2 : 0;

        var xPosition = (column * tileOffset.x) + oddRowOffset;
        var zPosition = row * tileOffset.y;

        var tilePos = new Vector3(xPosition, 0, zPosition);

        return tilePos;
    }

    private int2 PositionToGridIndex(Vector3 position)
    {
        var tileOffset = GetTileOffset();

        var xIndex = Mathf.FloorToInt(position.x / tileOffset.x);
        var yIndex = Mathf.RoundToInt(position.z / tileOffset.y);

        return new int2(xIndex, yIndex);
    }

    private Vector2 GetTileOffset()
    {
        var tileSlotRadius = (tilesMargin * tileRadius) + tileRadius;

        var xOffset = Mathf.Sqrt(3) * tileSlotRadius;
        var yOffset = tileSlotRadius * (3 / 2f);

        return new Vector2(xOffset, yOffset);
    }
}