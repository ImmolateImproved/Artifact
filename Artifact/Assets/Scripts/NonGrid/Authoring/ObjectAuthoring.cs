using Unity.Entities;
using UnityEngine;
using NonGrid.Components;

public class ObjectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public ObjectTypes objectType;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ObjectType { objectType = objectType });
    }
}