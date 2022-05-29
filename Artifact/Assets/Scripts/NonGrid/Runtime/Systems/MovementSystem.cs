using Latios;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using NonGrid.Components;
using UnityEngine;

public partial class MovementSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;
        var elapsedTime = Time.ElapsedTime;

        var settings = sceneBlackboardEntity.GetComponentData<AISettings>();

        Entities.ForEach((int entityInQueryIndex, ref Translation translation, in LocalToWorld ltw, in MoveSpeed moveSpeed) =>
        {
            translation.Value += ltw.Forward * moveSpeed.value * dt;

        }).ScheduleParallel();

        Entities.ForEach((int entityInQueryIndex, ref Translation translation, ref Rotation rotation, ref RotationChangeData rotationData, in LocalToWorld ltw) =>
        {
            var position = ltw.Position;

            if (position.x > settings.boundings.x || position.x < -settings.boundings.x)
            {
                translation.Value.x *= -1;
                translation.Value += ltw.Forward;

                //rotation.Value = quaternion.LookRotationSafe(-ltw.Forward, new float3(0, 1, 0));
                //translation.Value = float3.zero;
            }
            if (position.z > settings.boundings.y || position.z < -settings.boundings.y)
            {
                translation.Value.z *= -1;
                translation.Value += ltw.Forward;
                //rotation.Value = quaternion.LookRotationSafe(-ltw.Forward, new float3(0, 1, 0));
                //translation.Value = float3.zero;
            }

            var sin = (float)math.sin(elapsedTime) * math.radians(rotationData.speed) * dt;

            var currentDirection = quaternion.AxisAngle(new float3(0, 1, 0), sin);

            rotation.Value = math.mul(math.normalize(rotation.Value), currentDirection);

        }).ScheduleParallel();
    }
}