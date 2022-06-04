using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public partial class LookAroundSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.ForEach((ref DynamicBuffer<NeighborNode> neighborsBuffer, in IndexInGrid indexInGrid, in AliveStatus aliveStatus) =>
        {
            if (!aliveStatus.isAlive)
                return;
            
            var neighbors = grid.GetNeighborNodes(indexInGrid.current);

            neighborsBuffer.Reinterpret<int2>().CopyFrom(neighbors);

        }).Run();
    }
}