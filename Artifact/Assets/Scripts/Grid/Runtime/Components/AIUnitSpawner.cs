using Unity.Entities;

[GenerateAuthoringComponent]
public struct AIUnitSpawner : IComponentData
{
    public Entity prefab;
    public int count;
    public int minSpeed;
    public int maxSpeed;
}
