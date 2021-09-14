using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct UnitTag : IComponentData
{
    
}

public struct TileTag : IComponentData
{
    
}

public struct PathTile : IComponentData
{
    
}

public struct ActionRequest : IComponentData
{

}

public struct MousePosition : IComponentData
{
    public float2 value;
}

public struct DefaultColor : IComponentData
{
    public Color defaultColor;
}

public struct HoverColor : IComponentData
{
    public Color value;
}