using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TileTag : IComponentData
{
    
}

public struct PathTile : IComponentData
{
    
}

public struct DecisionRequest : IComponentData
{

}

public struct AttackTile : IComponentData
{
    
}

public struct MousePosition : IComponentData
{
    public float2 value;
}

public struct EntityColors : IComponentData
{
    public Color defaultColor;
}

public struct SelectionColors : IComponentData
{
    public Color hoveredColor;
    public Color selectedColor;
}

public struct CalculateMoveRange : IComponentData
{
    
}