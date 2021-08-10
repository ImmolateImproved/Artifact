using Unity.Entities;
using Latios;

public class ValidateExecutionSystem : SubSystem
{
    private EntityQuery validateExecutionQuery;

    protected override void OnCreate()
    {
        validateExecutionQuery = Fluent.WithAll<Moving>().WithAll<ExecutionRequest>().Build();
    }

    protected override void OnUpdate()
    {
        EntityManager.RemoveComponent<ExecutionRequest>(validateExecutionQuery);
    }
}