using Latios;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

[UpdateBefore(typeof(TransformSystemGroup))]
public class GameplayRootSuperSystem : RootSuperSystem
{
    protected override void CreateSystems()
    {
        GetOrCreateAndAddSystem<PlayerInputSuperSystem>();
        GetOrCreateAndAddSystem<PerFrameSuperSystem>();
        GetOrCreateAndAddSystem<SimulationTickSuperSystem>();
    }
}

public class PlayerInputSuperSystem : SuperSystem
{
    public override bool ShouldUpdateSystem()
    {
        return Input.GetMouseButtonDown(0);
    }

    protected override void CreateSystems()
    {
        GetOrCreateAndAddSystem<ClickSystem>();

        GetOrCreateAndAddSystem<UnitSelectionSystem>();
        GetOrCreateAndAddSystem<UnitSelectionViewSystem>();
        GetOrCreateAndAddSystem<RangeViewSystem>();
    }
}

public class SimulationTickSuperSystem : SuperSystem
{
    public float timer;

    public override bool ShouldUpdateSystem()
    {
        var simulationRate = GetSingleton<SimulationRate>().value;//sceneBlackboardEntity.GetComponentData<SimulationRate>();

        timer += Time.DeltaTime;

        var doTick = timer >= 1f / simulationRate;

        if (doTick)
        {
            timer = 0;
        }

        return doTick || Input.GetKeyDown(KeyCode.Space);
    }

    protected override void CreateSystems()
    {
        GetOrCreateAndAddSystem<LookAroundSystem>();
        GetOrCreateAndAddSystem<SelectDestinationSystem>();

        GetOrCreateAndAddSystem<AttackSystem>();
        GetOrCreateAndAddSystem<GridMovementSystem>();

        //GetOrCreateAndAddSystem<EnergySystem>();
        //GetOrCreateAndAddSystem<ReproductionSystem>();

        GetOrCreateAndAddSystem<RemoveDeadSystem>();
    }
}

public class PerFrameSuperSystem : SuperSystem
{
    protected override void CreateSystems()
    {
        //Initialization
        GetOrCreateAndAddSystem<GridInitializationSystem>();
        GetOrCreateAndAddSystem<UnitSpawnerSystem>();
        GetOrCreateAndAddSystem<UnitInitializationSystem>();

        GetOrCreateAndAddSystem<MouseHoverSystem>();
        GetOrCreateAndAddSystem<MouseHoverViewSystem>();
    }
}