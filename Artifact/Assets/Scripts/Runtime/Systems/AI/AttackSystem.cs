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

        Entities.WithAll<UnitTag>().ForEach((Entity e, ref IndexInGrid indexInGrid, in Destination destination) =>
        {
            var myEnergy = energyCDFE[e];

            if (myEnergy.energy < settings.attackEnergyPrice)
            {
                return;
            }

            var enemyNode = destination.node;

            var enemy = grid.GetGridObject(enemyNode);

            if (enemy == Entity.Null)
                return;

            var enemyEnergy = energyCDFE[enemy];

            var enemyIsValid = GetComponent<AliveStatus>(enemy).isAlive && GetComponent<UnitType>(enemy).value != UnitTypes.Coprse;

            if (!enemyIsValid)
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

        }).Run();
    }
}