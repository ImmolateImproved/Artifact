using Latios;
using Unity.Entities;
using Unity.Transforms;

public partial class ReproductionSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>(true);
        var settings = sceneBlackboardEntity.GetComponentData<Settings>();

        var ecb = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Translation, IndexInGrid, Energy>();

        Entities.ForEach((Entity e, ref Energy energy, in IndexInGrid indexInGrid, in Translation translation) =>
        {
            if (energy.energy < settings.energyForReproduction)
                return;

            var direction = HexDirections.BottomLeft;

            while (true)
            {
                direction = HexDirectionsExtentions.GetNextDirection(direction);

                var nextNode = grid.GetNeighborNodeFromDirection(indexInGrid.current, direction);

                if (grid.IsWalkable(nextNode))
                {
                    energy.energy -= settings.energyForReproduction;

                    var nodePosition = grid.GetNodePosition(nextNode);

                    nodePosition.y = translation.Value.y;

                    var newUnitPosition = new Translation { Value = nodePosition };
                    var newUnitGridIndex = new IndexInGrid { current = nextNode };
                    var newUnitEnergy = energy;
                    newUnitEnergy.energy = settings.initialEnergy;

                    ecb.Add(e, newUnitPosition, newUnitGridIndex, newUnitEnergy);

                    return;//Выход из цикла
                }
            }

        }).Run();
    }
}
