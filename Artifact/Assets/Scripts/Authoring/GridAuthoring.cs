using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

[DisallowMultipleComponent]
public class GridAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public int gridRadius;

    [Space]
    public float tileSize = 1;
    public float tilesMargin;

    public TileAuthoring tilePrefab;

    public TextMeshPro indicesTextPrefab;
    public bool showTileIndices;

    public bool showPath;

    public float TileSlotRadius
    {
        get => (tilesMargin * tileSize) + tileSize;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GridConfig(gridRadius, tileSize, TileSlotRadius));
    }

    public void GenerateGrid()
    {
        GenerateCircleGrid();
        SpawnTileText();
    }

    private void GenerateCircleGrid()
    {
        var mapHolder = CreateHolder("GeneratedMap");

        var nodes = BuildGridBFSAxial();

        foreach (var node in nodes)
        {
            var position = GridConfig.NodeToPosition(node, TileSlotRadius);
            position.y = -1;

            var tile = Instantiate(tilePrefab, position, Quaternion.identity, mapHolder);

            tile.transform.localScale = new Vector3(tileSize, 1, tileSize);

            tile.indexInGrid = node;
        }

        EditorUtility.SetDirty(mapHolder);
    }

    private HashSet<int2> BuildGridBFSAxial()
    {
        var neighbors = HexTileNeighbors.Neighbors;

        var tileInGridRadius = HexTileNeighbors.CalculateTilesCount(gridRadius);

        var queue = new Queue<int2>(tileInGridRadius);
        var visited = new HashSet<int2>();

        queue.Enqueue(0);
        visited.Add(0);

        while (visited.Count < tileInGridRadius)
        {
            var node = queue.Dequeue();

            for (int i = 0; i < neighbors.Length; i++)
            {
                var neighborNode = HexTileNeighbors.GetNeighborNode(node, neighbors[i]);

                if (visited.Add(neighborNode))
                {
                    queue.Enqueue(neighborNode);
                }
            }
        }

        return visited;
    }

    private void SpawnTileText()
    {
        if (!showTileIndices)
            return;

        var holderName = "GeneratedMap";
        var tilesParent = GameObject.Find(holderName);

        if (!tilesParent)
            return;

        var textParent = CreateHolder("Text");

        foreach (var tile in tilesParent.transform.GetComponentsInChildren<TileAuthoring>())
        {
            var textPos = new float3(tile.transform.position.x, 0.201f, tile.transform.position.z);

            var text = GameObject.Instantiate(indicesTextPrefab, textPos, Quaternion.Euler(90, 0, 0), textParent);

            var index = tile.indexInGrid;

            text.text = $"{index.x}.{index.y}";
        }
    }

    private Transform CreateHolder(string holderName)
    {
        var mapHolder = GameObject.Find(holderName);

        if (mapHolder)
        {
            DestroyImmediate(mapHolder);
        }

        mapHolder = new GameObject(holderName);

        SceneVisibilityManager.instance.DisablePicking(mapHolder, true);

        mapHolder.AddComponent<DisableOnStart>();

        return mapHolder.transform;
    }
}