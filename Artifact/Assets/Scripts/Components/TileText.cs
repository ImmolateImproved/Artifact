using Unity.Entities;
using TMPro;

[GenerateAuthoringComponent]
public class TileText : IComponentData
{
    public bool showTileIndices;
    public TextMeshPro textPrefab;
}