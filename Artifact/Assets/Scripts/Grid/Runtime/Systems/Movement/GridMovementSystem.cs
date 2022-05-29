using Latios;
using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class GridMovementSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        Entities.WithAll<Moving>()
            .ForEach((ref Translation translation, ref IndexInGrid indexInGrid, ref PreviousGridIndex previousGridIndex,
                    ref InDistance inDistance, in DestinationNode destinationNode, in MoveSpeed moveSpeed) =>
                    {
                        if (inDistance.value)
                        {
                            previousGridIndex.value = indexInGrid.value;
                            inDistance.value = false;
                        }

                        var position = destinationNode.position;
                        position.y = translation.Value.y;

                        translation.Value = MovementUtils.MoveTowards(translation.Value, position, moveSpeed.value * dt, out var distance);

                        if (distance < 0.01f)
                        {
                            indexInGrid.value = destinationNode.node;
                            inDistance.value = true;
                        }

                    }).Run();
    }
}