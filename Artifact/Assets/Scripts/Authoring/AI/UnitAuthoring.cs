using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class UnitAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public UnitTypes unitType;
    public bool selectable;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<UnitTag>(entity);
        dstManager.AddComponentData(entity, new UnitType { value = unitType });
        dstManager.AddComponent<IndexInGrid>(entity);

        if (selectable)
        {
            dstManager.AddComponent<Selectable>(entity);
        }
    }
}