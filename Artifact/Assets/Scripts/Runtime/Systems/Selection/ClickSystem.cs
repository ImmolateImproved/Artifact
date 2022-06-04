using Latios;
using Unity.Entities;

public partial class ClickSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.WithChangeFilter<HoverTile>()
            .ForEach((in HoverTile hoverTile) =>
            {
                var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

                if (hoverTile.current == Entity.Null)
                    return;

                EntityManager.AddComponentData(hoverTile.current, new Click());
                ecb.RemoveComponent<Click>(hoverTile.current);

            }).WithStructuralChanges().Run();
    }
}