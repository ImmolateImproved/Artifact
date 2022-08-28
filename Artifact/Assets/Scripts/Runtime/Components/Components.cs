using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct UnitTag : IComponentData
{

}

public struct UnitInitialized : IComponentData
{

}

public struct MousePosition : IComponentData
{
    public float2 value;
}

public struct HoverColor : IComponentData
{
    public Color value;
}

public struct RangeTileTag : IComponentData
{

}

public struct RangeTilePrefabRef : IComponentData
{
    public Entity prefab;
}