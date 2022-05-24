using Latios;
using Unity.Entities;

public partial class SelectMoveTargetSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.WithAll<ActionRequest>()
           .ForEach((Entity e, ref WaypointsMovement waypointsMovement, in DynamicBuffer<UnitPath> path) =>
           {
               if (path.Length == 0)
                   return;

               EntityManager.AddComponentData(e, new Moving());
               waypointsMovement.currentWaypointIndex = 0;

           }).WithStructuralChanges().Run();

        Entities.WithAll<Moving>()
           .ForEach((ref MoveDestination moveDestination, in WaypointsMovement waypointsMovement, in DynamicBuffer<UnitPath> path) =>
           {
               var currentNode = path[waypointsMovement.currentWaypointIndex].nodeIndex;

               moveDestination.node = currentNode;

           }).Run();
    }
}