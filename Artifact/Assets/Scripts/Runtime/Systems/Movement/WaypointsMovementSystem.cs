using Latios;
using Unity.Entities;

public partial class WaypointsMovementSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        Entities.WithAll<Moving>()
           .ForEach((Entity e, ref WaypointsMovement waypointsMovement, in MoveDestination moveDestination, in DynamicBuffer<UnitPath> path) =>
           {
               if (!moveDestination.inDistance) return;

               waypointsMovement.currentWaypointIndex++;

               if (waypointsMovement.currentWaypointIndex == path.Length)
               {
                   ecb.RemoveComponent<Moving>(e);
               }

           }).Run();
    }
}