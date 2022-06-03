using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class RangeViewSystem : SubSystem
{
    private EntityQuery moveRangeTileQuery;

    protected override void OnCreate()
    {
        moveRangeTileQuery = GetEntityQuery(typeof(RangeTileTag));
    }

    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var spawnECB = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Translation>();

        Entities.WithAll<SelectedInternal>().WithNone<Selected>()
            .ForEach(() =>
            {
                EntityManager.DestroyEntity(moveRangeTileQuery);

            }).WithStructuralChanges().Run();

        Entities.WithAll<Selected>().WithNone<SelectedInternal>()
            .ForEach((in IndexInGrid indexInGrid) =>
            {
                EntityManager.DestroyEntity(moveRangeTileQuery);

                var settings = sceneBlackboardEntity.GetComponentData<Settings>();

                var pathPrefab = sceneBlackboardEntity.GetComponentData<RangeTilePrefabRef>().prefab;

                var neighborsInRange = grid.GetNeighborsInRange(indexInGrid.current, settings.visionRange);

                for (int i = 0; i < neighborsInRange.Length; i++)
                {
                    var node = neighborsInRange[i];

                    var nodePos = grid.GetNodePosition(node);
                    var tilePos = new float3(nodePos.x, 0.05f, nodePos.z);

                    spawnECB.Add(pathPrefab, new Translation { Value = tilePos });
                }

            }).WithStructuralChanges().Run();
    }
}