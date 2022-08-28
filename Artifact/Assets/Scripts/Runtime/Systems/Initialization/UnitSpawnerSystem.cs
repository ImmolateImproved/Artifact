using Latios;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class UnitSpawnerSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

        var spawnECB = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Translation>();

        Entities.ForEach((Entity e, ref UnitSpawner spawner) =>
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
            Enabled = false;

        }).WithoutBurst().Run();
    }
}
