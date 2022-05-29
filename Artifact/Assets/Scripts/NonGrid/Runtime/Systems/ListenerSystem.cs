using Latios;
using NonGrid.Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class ListenerSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Rotation rotation, ref NotificationListener listener, in LocalToWorld ltw) =>
        {
            if (listener.changed)
            {
                rotation.Value = quaternion.LookRotationSafe(listener.notifierPosition - ltw.Position, new float3(0, 1, 0));

                listener.changed = false;
            }

        }).ScheduleParallel();
    }
}