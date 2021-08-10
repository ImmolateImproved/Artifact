using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

public struct SelectedUnit : IComponentData
{
    public Entity value;
}

public struct SelectionFilter : IComponentData
{
    public CollisionFilter collisionFilter;
}

public struct Hover : IComponentData
{

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