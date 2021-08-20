using Unity.Entities;
using Latios;
using Unity.Mathematics;
using Unity.Transforms;

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

        Entities.WithAll<Selected>()
            .ForEach((Entity e, ref PathRequestData pathRequest, ref AttackTarget attackTarget) =>
            {
                var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();
                var attackTileData = sceneBlackboardEntity.GetComponentData<AttackNodeManager>();

                var clickedTileIndex = clickedTileQuery.GetSingleton<IndexInGrid>().value;

                var destinationNode = grid.HasUnit(clickedTileIndex)
                ? attackTileData.node
                : clickedTileIndex;
                
                var moveRangeSet = EntityManager.GetCollectionComponent<MoveRangeSet>(e, true);

                if (!moveRangeSet.moveRangeHashSet.Contains(destinationNode))
                    return;

                pathRequest.target = destinationNode;

                attackTarget.node = clickedTileIndex;

                EntityManager.AddComponent<ExecutionRequest>(selectedQuery);
                ecb.RemoveComponent<ExecutionRequest>(selectedQuery);

            }).WithStoreEntityQueryInField(ref selectedQuery)
            .WithStructuralChanges().Run();
    }
}