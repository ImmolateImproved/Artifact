using Unity.Entities;

namespace NonGrid.Components
{
    public struct UnitTag : IComponentData { }

    public struct ObjectType : IComponentData
    {
        public ObjectTypes objectType;
    }

    public struct UnitsSpawned : IComponentData
    {

    }
}