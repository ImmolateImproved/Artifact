using Latios;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

public partial class MouseHoverSystem : SubSystem
{
    protected override void OnUpdate()
    {
        var tileGridData = sceneBlackboardEntity.GetCollectionComponent<TileGridData>();
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        Entities.ForEach((Entity e, in GridConfig gridConfig) =>
        {
            var plane = new Plane(Vector3.up, Vector3.zero);

            if (plane.Raycast(ray, out var rayDistance))
            {
                var point = ray.GetPoint(rayDistance);
                var node = gridConfig.PositionToNode(point);
                var tile = tileGridData.GetTile(node);

                var hoverTile = GetComponent<HoverTile>(e);//HoverTile не в ForEach, чтобы ChangeFilter сработал когда нужно

                if (tile != hoverTile.current)
                {
                    hoverTile.previous = hoverTile.current;
                    hoverTile.current = tile;
                    SetComponent(e, hoverTile);
                }
            }

        }).Run();
    }
}