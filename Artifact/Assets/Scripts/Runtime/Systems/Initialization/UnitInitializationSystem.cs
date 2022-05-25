using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public partial class UnitInitializationSystem : SubSystem
{
    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GridInitializedTag>();
    }

    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        var gridConfig = GetSingleton<GridConfig>();
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        var spawnECB = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Translation>();

        Entities.ForEach((Entity e, ref AIUnitSpawner spawner) =>
        {
            var gridEnumerator = grid.GetNodePositions();

            var count = math.min(spawner.count, grid.NodeCount);

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

            ecb.DestroyEntity(e);

        }).Run();

        Entities.WithAll<UnitTag>().WithNone<UnitInitialized>()
            .ForEach((Entity entity, ref Translation translation, ref IndexInGrid gridIndex, ref PreviousGridIndex previousGridIndex, in GridObjectType gridObjectType) =>
            {
                var node = gridConfig.PositionToNode(translation.Value);

                gridIndex.value = node;
                previousGridIndex.value = node;

                grid.SetGridObjects(node, entity);

                var positionResult = grid[node];

                if (positionResult != null)
                {
                    var position = positionResult.Value;

                    translation.Value = new float3(position.x, translation.Value.y, position.y);
                }

                ecb.AddComponent<UnitInitialized>(entity);

            }).Run();

        Entities.WithAll<UnitTag>().WithNone<UnitInitialized>()
            .ForEach((in UnitSelectionPointer selectionPointer) =>
            {
                ecb.AddComponent<Disabled>(selectionPointer.value);

            }).Run();
    }
}