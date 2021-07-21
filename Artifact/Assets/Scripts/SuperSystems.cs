using Latios;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

namespace Dragons
{
    [UpdateBefore(typeof(TransformSystemGroup))]
    public class GameplayRootSuperSystem : RootSuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddSystem<AISystem>();

            GetOrCreateAndAddSystem<PlayerInputSuperSystem>();

            GetOrCreateAndAddSystem<ExecutionSuperSystem>();

            GetOrCreateAndAddSystem<PerFrameSuperSystem>();
            GetOrCreateAndAddSystem<ReactiveSuperSystem>();
        }
    }

    public class PerFrameSuperSystem : SuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddSystem<BuildGridSystem>();
            GetOrCreateAndAddSystem<UnitInitializationSystem>();

            GetOrCreateAndAddSystem<MouseHoverSystem>();

            GetOrCreateAndAddSystem<WaypointsMovementSystem>();
            GetOrCreateAndAddSystem<UpdateGridSystem>();
        }
    }

    public class ReactiveSuperSystem : SuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddSystem<MouseHoverReactiveSystem>();
            GetOrCreateAndAddSystem<UnitSelectionReactiveSystem>();

            GetOrCreateAndAddSystem<MouseHoverViewSystem>();
            GetOrCreateAndAddSystem<PathfindingViewSystem>();
            GetOrCreateAndAddSystem<UnitSelectionViewSystem>();
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
            GetOrCreateAndAddSystem<TargetSelectionSystem>();
        }
    }

    public class ExecutionSuperSystem : SuperSystem
    {
        protected override void CreateSystems()
        {
            GetOrCreateAndAddSystem<ValidateExecutionSystem>();
            GetOrCreateAndAddSystem<PathfindingSystem>();
        }
    }
}