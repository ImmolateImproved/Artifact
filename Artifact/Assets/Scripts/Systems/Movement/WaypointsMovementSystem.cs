using Latios;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class WaypointsMovementSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var dt = Time.DeltaTime;

        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();

        var grid = latiosWorld.sceneBlackboardEntity.GetCollectionComponent<Grid>(true);

        Entities.WithAll<DecisionRequest>()
           .ForEach((Entity e, ref WaypointsMovement waypointsMovement, in DynamicBuffer<UnitPath> path) =>
           {
               if (path.Length == 0)
                   return;

               EntityManager.AddComponentData(e, new Moving());
               waypointsMovement.currentWaypointIndex = 0;

           }).WithStructuralChanges().Run();

        Entities.WithAll<Moving>()
            .ForEach((Entity e, ref Translation translation, ref WaypointsMovement waypointsMovement, ref IndexInGrid indexInGrid, in MoveSpeed moveSpeed, in DynamicBuffer<UnitPath> path) =>
            {
                var currentNode = path[waypointsMovement.currentWaypointIndex].nodeIndex;

                var position = grid[currentNode];

                var nextPosition = new float3(position.x, translation.Value.y, position.y);

                translation.Value = MoveTowards(translation.Value, nextPosition, moveSpeed.value * dt);

                var distance = math.distance(translation.Value, nextPosition) < 0.01f;

                if (distance)
                {
                    indexInGrid.value = currentNode;
                    waypointsMovement.currentWaypointIndex++;

                    if (waypointsMovement.currentWaypointIndex == path.Length)
                    {
                        ecb.RemoveComponent<Moving>(e);
                    }
                }

            }).Run();
    }

    public static float3 MoveTowards(float3 current, float3 target, float maxDistanceDelta)
    {
        float3 a = target - current;
        float magnitude = math.length(a);
        if (magnitude <= maxDistanceDelta || magnitude == 0f)
        {
            return target;
        }
        return current + a / magnitude * maxDistanceDelta;
    }
}