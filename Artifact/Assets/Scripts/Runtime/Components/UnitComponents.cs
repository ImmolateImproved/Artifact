using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Unity.Collections;
using System;

public enum UnitTypes
{
    LivingEntity, Grass, Coprse
}

public struct UnitType : IComponentData
{
    public UnitTypes value;
}