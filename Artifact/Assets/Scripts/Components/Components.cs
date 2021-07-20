using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TileTag : IComponentData
{
    
}

public struct PathTile : IComponentData
{
    
}

public struct ExecutionRequest : IComponentData
{

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