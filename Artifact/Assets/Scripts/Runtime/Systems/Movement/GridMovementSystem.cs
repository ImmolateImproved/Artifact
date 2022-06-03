﻿using Latios;
using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public partial class GridMovementSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<Moving>()
            .ForEach((ref Translation translation, ref IndexInGrid indexInGrid, in DestinationNode destinationNode, in AliveStatus aliveStatus) =>
                    {
                        if (!aliveStatus.isAlive)
                            return;

                        indexInGrid.current = destinationNode.node;

                        var position = destinationNode.position;
                        position.y = translation.Value.y;

                        translation.Value = position;

                    }).Run();
    }
}