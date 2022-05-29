using Latios;
using Unity.Entities;
using Unity.Transforms;
using NonGrid.Components;
using Unity.Mathematics;

public partial class ResourceSeekerSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref AIData aIData, ref Rotation rotation, ref NeighborObject neighborObject, in LocalToWorld ltw) =>
        {
            aIData.stepsToBase++;
            aIData.stepsToResource++;

            if (neighborObject.objectType == ObjectTypes.None)
                return;

            if (aIData.target != neighborObject.objectType)
                return;

            if (aIData.target == ObjectTypes.Base)
            {
                aIData.stepsToBase = 0;
                aIData.target = ObjectTypes.Resource;
            }
            else if (aIData.target == ObjectTypes.Resource)
            {
                aIData.stepsToResource = 0;
                aIData.target = ObjectTypes.Base;
            }

            neighborObject.objectType = ObjectTypes.None;
            rotation.Value = quaternion.LookRotationSafe(-ltw.Forward, new float3(0, 1, 0));

        }).ScheduleParallel();
    }
}