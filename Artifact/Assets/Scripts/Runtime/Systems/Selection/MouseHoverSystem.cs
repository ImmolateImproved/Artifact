using Latios;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class MouseHoverSystem : SubSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach((in GridConfig gridConfig) =>
        {
            var grid = sceneBlackboardEntity.GetCollectionComponent<Grid>();

            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out var rayDistance))
            {
                var point = ray.GetPoint(rayDistance);
                var node = gridConfig.PositionToNode(point);
                var tile = grid.GetTile(node);

                sceneBlackboardEntity.SetComponentData(new MousePosition { value = new float2(point.x, point.z) });

                var isHoverEntityExisting = TryGetSingletonEntity<Hover>(out var hoverEntity);

                if (hoverEntity != tile)
                {
                    if (isHoverEntityExisting)
                    {
                        EntityManager.RemoveComponent<Hover>(hoverEntity);
                    }

                    if (tile != Entity.Null)
                    {
                        EntityManager.AddComponentData(tile, new Hover());
                    }
                }
            }

        }).WithStructuralChanges().Run();
    }
}