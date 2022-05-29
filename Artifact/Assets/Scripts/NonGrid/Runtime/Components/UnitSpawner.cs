using Unity.Entities;

namespace NonGrid.Components
{
    [GenerateAuthoringComponent]
    public struct UnitSpawner : IComponentData
    {
        public Entity prefab;
        public int count;
    }
}