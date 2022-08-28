using Unity.Entities;

[GenerateAuthoringComponent]
public struct UnitSpawner : IComponentData
{
    public Entity prefab;
    public int count;
}
