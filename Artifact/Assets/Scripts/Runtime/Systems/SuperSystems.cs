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

        GetOrCreateAndAddSystem<ActionRequestSystem>();
    }
}

public class PerFrameSuperSystem : SuperSystem
{
    protected override void CreateSystems()
    {
        //Initialization
        GetOrCreateAndAddSystem<GridInitializationSystem>();
        GetOrCreateAndAddSystem<UnitInitializationSystem>();
        //

        GetOrCreateAndAddSystem<MouseHoverSystem>();
        GetOrCreateAndAddSystem<MouseHoverReactiveSystem>();

        GetOrCreateAndAddSystem<TargetSelectionSystem>();

        GetOrCreateAndAddSystem<PathfindingSystem>();
        //Movement

        GetOrCreateAndAddSystem<SelectMoveTargetSystem>();
        GetOrCreateAndAddSystem<MovementSystem>();
        GetOrCreateAndAddSystem<IncrementWaypointIndexSystem>();

        GetOrCreateAndAddSystem<UpdateGridSystem>();

        //AI Systems
        //GetOrCreateAndAddSystem<AINotifierSystem>();
        GetOrCreateAndAddSystem<AIObserverSystem>();
        //GetOrCreateAndAddSystem<AIListenerSystem>();

        GetOrCreateAndAddSystem<UpdatePreviousGridIndexSystem>();
        //
        GetOrCreateAndAddSystem<MovementReactionSystem>();
        GetOrCreateAndAddSystem<CalculateMoveRangeSystem>();
        //

        #region ViewSystems
        //View
        GetOrCreateAndAddSystem<MouseHoverViewSystem>();
        GetOrCreateAndAddSystem<AttackTargetSelectionViewSystem>();
        GetOrCreateAndAddSystem<PathfindingViewSystem>();
        GetOrCreateAndAddSystem<UnitSelectionViewSystem>();
        GetOrCreateAndAddSystem<MoveRangeViewSystem>();

        #endregion
    }
}