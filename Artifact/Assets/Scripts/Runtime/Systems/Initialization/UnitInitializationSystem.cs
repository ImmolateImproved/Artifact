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

        var spawnECB = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Translation, MoveSpeed>();

        Entities.ForEach((Entity e, ref AIUnitSpawner spawner) =>
        {
            var gridEnumerator = grid.GetAllNodePositions();

            var count = math.min(spawner.count, grid.NodeCount);

            while (gridEnumerator.MoveNext())
            {
                if (count-- <= 0)
                    break;

                if (!grid.IsWalkable(gridEnumerator.Current.Key))
                    continue;

                var nodePos = gridEnumerator.Current.Value;

                var position = new float3(nodePos.x, 0.75f, nodePos.y);

                spawnECB.Add(spawner.prefab, new Translation { Value = position },
                                             new MoveSpeed { value = UnityEngine.Random.Range(spawner.minSpeed, spawner.maxSpeed) });
            }

            ecb.DestroyEntity(e);

        }).Run();

        Entities.WithNone<UnitInitialized>()
            .ForEach((Entity entity, ref Translation translation, ref IndexInGrid gridIndex, ref PreviousGridIndex previousGridIndex) =>
            {
                var node = gridConfig.PositionToNode(translation.Value);

                if (grid.HasNode(node))
                {
                    gridIndex.value = node;
                    previousGridIndex.value = node;

                    grid.SetGridObjects(node, entity);

                    var position = grid.GetNodePosition(node);
                    position.y = translation.Value.y;

                    translation.Value = position;
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