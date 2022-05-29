using Latios;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using NonGrid.Components;
using UnityEngine;

public partial class UnitSpawnerSystem : SubSystem
{
    private Rng m_rng;

    public override void OnNewScene()
    {
        m_rng = new Rng("UnitSpawnerSystem");
    }

    protected override void OnUpdate()
    {
        var ecb = latiosWorld.syncPoint.CreateEntityCommandBuffer();
        var spawnEcb = latiosWorld.syncPoint.CreateInstantiateCommandBuffer<Translation, MoveSpeed>();

        var rng = m_rng;

        Entities.WithNone<UnitsSpawned>()
            .ForEach((Entity e, int entityInQueryIndex, in UnitSpawner spawner, in AISettings aISettings) =>
            {
                var random = rng.GetSequence(entityInQueryIndex);

                var maxPos = new float3(aISettings.boundings.x, 0.5f, aISettings.boundings.y);
                var minPos = new float3(-aISettings.boundings.x, 0.5f, -aISettings.boundings.y);

                for (int i = 0; i < spawner.count; i++)
                {
                    var position = random.NextFloat3(minPos, maxPos);

                    var speed = random.NextFloat(aISettings.minUnitSpeed, aISettings.maxUnitSpeed);

                    spawnEcb.Add(spawner.prefab, new Translation { Value = position }, new MoveSpeed { value = speed });
                }

                ecb.AddComponent<UnitsSpawned>(e);

            }).Schedule();

        m_rng.Shuffle();
    }
}