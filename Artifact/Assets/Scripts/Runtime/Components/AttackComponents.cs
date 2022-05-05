using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct TargetManager : IComponentData
{
    public int2? moveTarget;
    public int2? attackTarget;
}

public struct AttackNodeView : IComponentData
{
    public Entity attackPointerPrefab;

    public Entity attackPointerEntity;

    public int2 moveNode;
    public int2 attackTarget;
}

public struct AttackTarget : IComponentData
{
    public int2 node;
}

public struct AttackState : IComponentData
{
    public bool attack;
}

public struct AttackRequest : IComponentData
{
    
}