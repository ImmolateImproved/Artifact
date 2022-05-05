using Latios;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public partial class InitializeGridSystem : SubSystem
{
    private EntityQuery gridConfigQuery;

    public override bool ShouldUpdateSystem()
    {
        return !gridConfigQuery.IsEmptyIgnoreFilter;
    }

    protected override void OnUpdate()
    {
        var config = default(GridConfiguration);
        //Get config data
        Entities.ForEach((in GridConfiguration gridConfig) =>
        {
            config = gridConfig;

        }).WithStoreEntityQueryInField(ref gridConfigQuery).Run();

        EntityManager.RemoveComponent<GridConfiguration>(gridConfigQuery);

        //Initialize Grid Collection Component
        var grid = new Grid(config, Allocator.Persistent);
        sceneBlackboardEntity.AddCollectionComponent(grid);

        //Initialize units grid index
        Entities.WithAll<UnitTag>().WithNone<MoveRangeAssociated>()
            .ForEach((Entity e, in Translation translation) =>
            {
                var node = grid.PositionToNode(translation.Value);

                EntityManager.AddComponentData(e, new IndexInGrid { value = node });
                EntityManager.AddComponentData(e, new PreviousGridIndex { value = node });

            }).WithStructuralChanges().Run();

        //Initialize tile positions
        Entities.WithAll<TileTag>()
             .ForEach((in Translation translation, in IndexInGrid indexInGrid) =>
             {
                 grid[indexInGrid.value] = new float2(translation.Value.x, translation.Value.z);

             }).Run();

        //Initialize "Index to tile-entity" HashMap
        Entities.WithAll<TileTag>()
            .ForEach((Entity entity, in IndexInGrid gridIndex) =>
            {
                grid.InitTile(gridIndex.value, entity);

            }).Run();

        //Initialize units position
        Entities.WithAll<UnitTag>()
            .ForEach((Entity entity, ref Translation translation, in IndexInGrid gridIndex) =>
            {
                grid.SetUnit(gridIndex.value, entity);

                var positionResult = grid[gridIndex.value];

                if (positionResult != null)
                {
                    var position = positionResult.Value;

                    translation.Value = new float3(position.x, translation.Value.y, position.y);
                }

            }).Run();
    }
}