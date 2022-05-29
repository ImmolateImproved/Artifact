using Latios;
using Latios.Psyshock;
using Unity.Collections;
using Unity.Entities;
using NonGrid.Components;

public class FindNeighborObjectsSystem : SubSystem
{
    private EntityQuery unitQuery, objectQuery;

    protected override void OnCreate()
    {
        unitQuery = Fluent.WithAll<UnitTag>().PatchQueryForBuildingCollisionLayer().Build();
        objectQuery = Fluent.WithAll<ObjectType>().PatchQueryForBuildingCollisionLayer().Build();
    }

    protected override void OnUpdate()
    {
        Dependency = Physics.BuildCollisionLayer(unitQuery, this).ScheduleParallel(out var unitsLayer, Allocator.TempJob, Dependency);
        Dependency = Physics.BuildCollisionLayer(objectQuery, this).ScheduleParallel(out var objectsLayer, Allocator.TempJob, Dependency);

        var processor = new FindNeighborsProcessor
        {
            neighborObjectCDFE = GetComponentDataFromEntity<NeighborObject>(),
            objectCDFE = GetComponentDataFromEntity<ObjectType>()
        };

        Dependency = Physics.FindPairs(unitsLayer, objectsLayer, processor).ScheduleParallel(Dependency);

        Dependency = unitsLayer.Dispose(Dependency);
        Dependency = objectsLayer.Dispose(Dependency);
    }

    private struct FindNeighborsProcessor : IFindPairsProcessor
    {
        public PhysicsComponentDataFromEntity<NeighborObject> neighborObjectCDFE;
        public PhysicsComponentDataFromEntity<ObjectType> objectCDFE;

        public void Execute(FindPairsResult result)
        {
            if (Physics.DistanceBetween(result.bodyA.collider, result.bodyA.transform, result.bodyB.collider, result.bodyB.transform, 0f, out _))
            {
                var objectType = objectCDFE[result.entityB].objectType;

                neighborObjectCDFE[result.entityA] = new NeighborObject
                {
                    objectType = objectType
                };
            }
        }
    }
}