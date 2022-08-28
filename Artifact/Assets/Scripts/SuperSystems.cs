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

public class PerFrameSuperSystem : SuperSystem
{
    protected override void CreateSystems()
    {
        //Initialization
        GetOrCreateAndAddSystem<GridInitializationSystem>();
        GetOrCreateAndAddSystem<UnitSpawnerSystem>();
        GetOrCreateAndAddSystem<UnitInitializationSystem>();

        //GetOrCreateAndAddSystem<GridMovementSystem>();

        GetOrCreateAndAddSystem<MouseHoverSystem>();
        GetOrCreateAndAddSystem<MouseHoverViewSystem>();
    }
}