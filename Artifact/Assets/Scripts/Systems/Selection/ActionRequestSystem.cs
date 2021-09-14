using Unity.Entities;
using Latios;

public class ActionRequestSystem : SubSystem
{
    private EntityQuery clickedTileQuery;
    private EntityQuery selectedQuery;

    protected override void OnCreate()
    {
        clickedTileQuery = GetEntityQuery(typeof(Click), typeof(TileTag), typeof(IndexInGrid));
    }

    public override bool ShouldUpdateSystem()
    {
        return !clickedTileQuery.IsEmpty && HasSingleton<Selected>() && UnityEngine.Input.GetMouseButtonDown(1);
    }

    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithAll<Selected>().WithNone<Moving>()
            .ForEach((Entity e, ref PathfindingTarget pathFindingTarget, ref AttackTarget attackTarget, ref AttackState attackState) =>
            {
                var targetData = sceneBlackboardEntity.GetComponentData<TargetManager>();

                if (targetData.moveTarget.Equals(-1))
                    return;

                pathFindingTarget.node = targetData.moveTarget;

                attackState.attack = !targetData.attackTarget.Equals(-1);
                attackTarget.node = targetData.attackTarget;

                EntityManager.AddComponent<ActionRequest>(selectedQuery);
                ecb.RemoveComponentForEntityQuery<ActionRequest>(selectedQuery);

            }).WithStoreEntityQueryInField(ref selectedQuery)
            .WithStructuralChanges().Run();
    }
}