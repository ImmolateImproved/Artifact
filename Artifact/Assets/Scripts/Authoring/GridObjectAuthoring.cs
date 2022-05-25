using Unity.Entities;
using UnityEngine;

public class GridObjectAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public GridObjectTypes objectType;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GridObjectType
        {
            value = objectType
        });

        switch (objectType)
        {
            case GridObjectTypes.Base:
                {
                    dstManager.AddComponent<BaseTag>(entity);
                    break;
                }

            case GridObjectTypes.Recource:
                {
                    dstManager.AddComponent<Resource>(entity);
                    break;
                }
        }
    }
}