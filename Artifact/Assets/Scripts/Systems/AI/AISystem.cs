using Latios;
using Unity.Entities;
using Unity.Mathematics;

public class AISystem : SubSystem
{
    private Random random;

    protected override void OnCreate()
    {
        random = new Random();
        random.InitState();
    }

    public override bool ShouldUpdateSystem()
    {
        return UnityEngine.Input.GetKeyDown(UnityEngine.KeyCode.Space);
    }

    protected override void OnUpdate()
    {
        var map = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.ForEach((Entity e, in DynamicBuffer<AIUnits> aIUnits) =>
        {
            var gridPos = random.NextInt2(int2.zero, new int2(map.columns, map.rows));

            //if (!map.IsWalkable(gridPos))
            //    return;

            var unit = aIUnits[0].entity;

            EntityManager.SetComponentData(unit, new PathfindingTarget { node = gridPos });

            EntityManager.AddComponentData(unit, new DecisionRequest());
            ecb.RemoveComponent<DecisionRequest>(unit);

        }).WithStructuralChanges().Run();
    }
}