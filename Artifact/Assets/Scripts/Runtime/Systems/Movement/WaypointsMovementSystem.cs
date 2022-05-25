using Latios;
using Unity.Entities;

public partial class WaypointsMovementSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithAll<Moving>()
           .ForEach((Entity e, ref WaypointsMovement waypointsMovement, ref DynamicBuffer<UnitPath> path, in MoveDestination moveDestination) =>
           {
               if (!moveDestination.inDistance)
                   return;

               waypointsMovement.currentWaypointIndex++;

               if (waypointsMovement.currentWaypointIndex == path.Length)
               {
                   path.Length = 0;
                   // ecb.RemoveComponent<Moving>(e);
               }

           }).Run();
    }
}