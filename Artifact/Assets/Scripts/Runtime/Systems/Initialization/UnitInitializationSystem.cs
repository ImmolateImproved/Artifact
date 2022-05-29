using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class UnitInitializationSystem : SubSystem
{
    public EntityQuery nonInitializedUnitsQuery;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GridInitializedTag>();

        RequireForUpdate(nonInitializedUnitsQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        var gridConfig = sceneBlackboardEntity.GetComponentData<GridConfig>();
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        var spawnECB = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Translation>();

        Entities.ForEach((Entity e, ref AIUnitSpawner spawner) =>
        {
            var gridEnumerator = grid.GetAllNodePositions();

            var count = math.min(spawner.count, grid.NodeCount);
            spawner.count = 0;

            while (gridEnumerator.MoveNext())
            {
                if (count-- <= 0)
                    break;

                if (!grid.IsWalkable(gridEnumerator.Current.Key))
                    continue;

                var nodePos = gridEnumerator.Current.Value;

                var position = new float3(nodePos.x, 0.75f, nodePos.y);

                spawnECB.Add(spawner.prefab, new Translation { Value = position });
            }

        }).Run();

        Entities.WithNone<UnitInitialized>()
            .ForEach((Entity entity, ref Translation translation, ref IndexInGrid gridIndex) =>
            {
                var node = gridConfig.PositionToNode(translation.Value);

                if (grid.HasNode(node))
                {
                    gridIndex.current = node;
                    gridIndex.previous = node;

                    grid.SetGridObjects(node, entity);

                    var position = grid.GetNodePosition(node);
                    position.y = translation.Value.y;

                    translation.Value = position;
                }

                ecb.AddComponent<UnitInitialized>(entity);

            }).WithStoreEntityQueryInField(ref nonInitializedUnitsQuery).Run();

        Entities.WithAll<UnitTag>().WithNone<UnitInitialized>()
            .ForEach((in UnitSelectionPointer selectionPointer) =>
            {
                ecb.AddComponent<Disabled>(selectionPointer.value);

            }).Run();
    }
}