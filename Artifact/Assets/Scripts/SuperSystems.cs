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
        return Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    }

    protected override void CreateSystems()
    {
        GetOrCreateAndAddSystem<ClickSystem>();

        GetOrCreateAndAddSystem<UnitSelectionSystem>();
        GetOrCreateAndAddSystem<UnitSelectionReactiveSystem>();
        GetOrCreateAndAddSystem<RangeViewSystem>();
    }
}

public class SimulationTickSuperSystem : SuperSystem
{
    public override bool ShouldUpdateSystem()
    {
        return Input.GetKeyDown(KeyCode.Space);
    }

    protected override void CreateSystems()
    {
        GetOrCreateAndAddSystem<AttackSystem>();

        //Movement
        GetOrCreateAndAddSystem<SelectDestinationSystem>();
        GetOrCreateAndAddSystem<GridMovementSystem>();
        //

        GetOrCreateAndAddSystem<EnergySystem>();
        GetOrCreateAndAddSystem<ReproductionSystem>();

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
        #region ViewSystems
        ////View
        GetOrCreateAndAddSystem<MouseHoverViewSystem>();
        GetOrCreateAndAddSystem<UnitSelectionViewSystem>();

        #endregion
    }
}