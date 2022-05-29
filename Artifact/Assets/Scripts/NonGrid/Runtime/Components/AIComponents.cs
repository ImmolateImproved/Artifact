using Latios;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;

namespace NonGrid.Components
{
    public enum ObjectTypes
    {
        None, Base, Resource
    }

    public struct AISettings : IComponentData
    {
        public float searchRadius;

        public float minUnitSpeed;
        public float maxUnitSpeed;

        public float2 boundings;

        public CollisionFilter collisionFilter;
    }

    public struct AIData : IComponentData
    {
        public int stepsToBase;
        public int stepsToResource;

        public ObjectTypes target;
    }

    public struct NotificationListener : IComponentData
    {
        public int stepsToBase;
        public int stepsToResource;

        public float3 notifierPosition;
        public bool changed;
    }

    public struct NeighborUnit : IBufferElementData
    {
        public EntityWith<NotificationListener> value;
    }

    public struct NeighborObject : IComponentData
    {
        public ObjectTypes objectType;
    }
}