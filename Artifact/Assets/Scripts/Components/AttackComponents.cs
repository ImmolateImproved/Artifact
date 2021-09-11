using TMPro;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public struct AttackNodeManager : IComponentData
{
    public int2 node;
}

public struct AttackNodeView : IComponentData
{
    public Entity attackPointerPrefab;

    public Entity attackPointerEntity;
    public int2 attackNode;
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

//class components
public class UnitCombat : IComponentData
{
    public UnitCombatBehaviour combatBehaviour;

    public UnitCombat()
    {

    }
}