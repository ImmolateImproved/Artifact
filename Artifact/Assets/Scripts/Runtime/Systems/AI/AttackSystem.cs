using Latios;
using Unity.Entities;
using UnityEngine;

public partial class AttackSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();
        var settings = sceneBlackboardEntity.GetComponentData<Settings>();

        var energyCDFE = GetComponentDataFromEntity<Energy>();

        Entities.WithAll<UnitTag>().ForEach((Entity e, ref IndexInGrid indexInGrid) =>
        {
            var myEnergy = energyCDFE[e];

            if (myEnergy.energy < settings.attackEnergyPrice)
            {
                return;
            }

            var neighbors = grid.FindGridObjects(indexInGrid.current, 1);

            if (neighbors.Length == 0)
                return;

            var enemyNode = neighbors[0];

            if (enemyNode.Equals(indexInGrid.current))
                return;

            var enemy = grid.GetGridObject(enemyNode);

            var enemyEnergy = energyCDFE[enemy];

            if (GetComponent<UnitType>(enemy).value == UnitTypes.Coprse)
                return;

            if (!GetComponent<AliveStatus>(enemy).isAlive)
                return;

            if (myEnergy.energy >= enemyEnergy.energy)
            {
                myEnergy.energy += enemyEnergy.energy * myEnergy.digestibility;

                SetComponent(enemy, new AliveStatus { isAlive = false });
                grid.RemoveGridObject(enemyNode);
            }
            else
            {
                SetComponent(e, new AliveStatus { isAlive = false });
                grid.RemoveGridObject(indexInGrid.current);
            }

            energyCDFE[e] = myEnergy;
            energyCDFE[enemy] = enemyEnergy;

        }).WithoutBurst().Run();
    }
}