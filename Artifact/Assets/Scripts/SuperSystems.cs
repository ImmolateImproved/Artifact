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

        GetOrCreateAndAddSystem<TargetSelectionSystem>();
    }
}

public class PerFrameSuperSystem : SuperSystem
{
    protected override void CreateSystems()
    {
        //Initialization
        GetOrCreateAndAddSystem<BuildGridSystem>();
        GetOrCreateAndAddSystem<UnitInitializationSystem>();
        //

        GetOrCreateAndAddSystem<MouseHoverSystem>();
        GetOrCreateAndAddSystem<MouseHoverReactiveSystem>();

        GetOrCreateAndAddSystem<AttackNodeSelectionSystem>();

        //Movement
        GetOrCreateAndAddSystem<PathfindingSystem>();
        GetOrCreateAndAddSystem<WaypointsMovementSystem>();
        GetOrCreateAndAddSystem<MovementReactionSystem>();
        GetOrCreateAndAddSystem<UpdateGridSystem>();
        //

        GetOrCreateAndAddSystem<AttackSystem>();

        #region ViewSystems
        //View
        GetOrCreateAndAddSystem<MouseHoverViewSystem>();
        GetOrCreateAndAddSystem<AttackNodeViewSystem>();
        GetOrCreateAndAddSystem<PathfindingViewSystem>();
        GetOrCreateAndAddSystem<UnitSelectionViewSystem>();
        GetOrCreateAndAddSystem<CalculateMoveRangeSystem>();
        GetOrCreateAndAddSystem<MoveRangeViewSystem>();

        #endregion
    }
}