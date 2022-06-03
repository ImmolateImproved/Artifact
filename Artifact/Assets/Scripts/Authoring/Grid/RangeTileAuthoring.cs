﻿using Unity.Entities;
using UnityEngine;

public class RangeTileAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RangeTileTag { });
    }
}