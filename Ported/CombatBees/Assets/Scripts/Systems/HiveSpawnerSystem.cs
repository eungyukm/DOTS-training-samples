using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Rendering;
using Unity.Transforms;

namespace Systems
{
    public partial class HiveSpawnerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            var random = new Random(1234);

            var hiveSpawner = GetSingletonEntity<HiveSpawner>();
            
            Entities
                .ForEach((Entity entity, in HiveSpawner spawner, in Translation worldCenter, in NonUniformScale worldSize ) =>
                {
                    ecb.RemoveComponent<HiveSpawner>(entity);
                    // ecb.DestroyEntity(entity);
                    var worldStartPosition =(worldCenter.Value - worldSize.Value/2);
                    for (int i = 0; i < spawner.BeesAmount; ++i)
                    {
                        var bee = ecb.Instantiate(spawner.BeePrefab);
                        var translation = new Translation { Value = worldStartPosition + worldSize.Value * random.NextFloat3(1) };
                        ecb.SetComponent(bee, translation);
                    }
                    
                    for (int i = 0; i < spawner.ResourceAmount; ++i)
                    {
                        var bee = ecb.Instantiate(spawner.ResourcePrefab);
                        var translation = new Translation { Value = worldStartPosition + new float3(worldSize.Value.x, 0, worldSize.Value.z) * random.NextFloat3(1) };
                        ecb.SetComponent(bee, translation);
                    }
                }).Run();

            ecb.Playback(EntityManager);
            ecb.Dispose();
        }
    }
}