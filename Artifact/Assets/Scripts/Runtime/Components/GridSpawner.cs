using Unity.Entities;

[GenerateAuthoringComponent]
public struct GridSpawner : IComponentData
{
    public Entity prefab;
}