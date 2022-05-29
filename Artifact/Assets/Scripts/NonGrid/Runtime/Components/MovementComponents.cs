using Latios;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;

namespace NonGrid.Components
{
    public struct RotationChangeData : IComponentData
    {
        public float speed;
    }

    public struct MoveSpeed : IComponentData
    {
        public float value;
    }
}