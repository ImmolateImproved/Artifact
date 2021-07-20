using Unity.Entities;
using Latios;
using Unity.Mathematics;

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
            .ForEach((Entity e, ref PathRequestData pathRequest) =>
            {
                pathRequest.target = clickedTileQuery.GetSingleton<IndexInGrid>().value;

                EntityManager.AddComponent<ExecutionRequest>(selectedQuery);
                ecb.RemoveComponent<ExecutionRequest>(selectedQuery);

            }).WithStoreEntityQueryInField(ref selectedQuery)
            .WithStructuralChanges().Run();
    }
}