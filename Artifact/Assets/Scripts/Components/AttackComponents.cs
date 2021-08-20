using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AttackNodeManager : IComponentData
{
    public int2 node;
}

public struct AttackNodeView : IComponentData
{
    public Entity attackTilePrefab;

    public Entity attackTileEntity;
    public int2 attackNode;
}

public struct AttackTarget : IComponentData
{
    public int2 node;
}