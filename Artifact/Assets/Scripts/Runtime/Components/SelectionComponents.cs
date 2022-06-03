using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct SelectedUnit : IComponentData
{
    public Entity value;
}

public struct UnitSelectionPointer : IComponentData
{
    public Entity value;
    public float yPosition;
}

public struct HoverTile : IComponentData
{
    public Entity current;
    public Entity previous;
}

public struct Click : IComponentData
{

}

public struct Selected : IComponentData
{

}

public struct Selectable : IComponentData
{

}