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

        Entities.ForEach((Entity e, ref Energy energy, ref MoveDirection moveDirection, in IndexInGrid indexInGrid, in Translation translation) =>
        {
            if (energy.energy < settings.energyForReproduction)
                return;

            while (true)
            {
                moveDirection.value = HexDirectionsExtentions.GetNextDirection(moveDirection.value);

                var nextNode = grid.GetNeighborNodeFromDirection(indexInGrid.current, moveDirection.value);

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
