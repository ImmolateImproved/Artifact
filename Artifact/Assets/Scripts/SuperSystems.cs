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
        GetOrCreateAndAddSystem<GridMovementPerFrameSuperSystem>();
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
    }
}

public class GridMovementPerFrameSuperSystem : SuperSystem
{
    protected override void CreateSystems()
    {
        //Initialization
        GetOrCreateAndAddSystem<GridInitializationSystem>();
        GetOrCreateAndAddSystem<UnitInitializationSystem>();
        //

        GetOrCreateAndAddSystem<MouseHoverSystem>();
        GetOrCreateAndAddSystem<MouseHoverReactiveSystem>();

        //
        GetOrCreateAndAddSystem<PathfindingSystem>();

        GetOrCreateAndAddSystem<GridMovementSystem>();

        GetOrCreateAndAddSystem<UpdateGridSystem>();

        #region ViewSystems
        //View
        GetOrCreateAndAddSystem<MouseHoverViewSystem>();
        GetOrCreateAndAddSystem<UnitSelectionViewSystem>();

        #endregion
    }
}