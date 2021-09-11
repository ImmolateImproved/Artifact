using Unity.Entities;
using Latios;

public class TargetSelectionSystem : SubSystem
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
                var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();
                var attackTileData = sceneBlackboardEntity.GetComponentData<AttackNodeManager>();

                var clickedTileIndex = clickedTileQuery.GetSingleton<IndexInGrid>().value;

                var hasUnit = grid.HasUnit(clickedTileIndex);

                var destinationNode = hasUnit
                ? attackTileData.node
                : clickedTileIndex;

                var moveRangeSet = EntityManager.GetCollectionComponent<MoveRangeSet>(e, true);

                if (!moveRangeSet.moveRangeHashSet.Contains(destinationNode))
                    return;

                pathFindingTarget.node = destinationNode;

                attackState.attack = hasUnit;
                attackTarget.node = clickedTileIndex;

                EntityManager.AddComponent<DecisionRequest>(selectedQuery);
                ecb.RemoveComponentForEntityQuery<DecisionRequest>(selectedQuery);

            }).WithStoreEntityQueryInField(ref selectedQuery)
            .WithStructuralChanges().Run();
    }
}