using Unity.Entities;
using Unity.Physics;

public struct SelectionManager : IComponentData
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