using Components;
using Jobs;
using Unity.Entities;

namespace Systems
{
    [UpdateAfter(typeof(NavAgentSystem))]
    public partial struct MovingSystem : ISystem
    {
        private BufferLookup<WaypointBuffer> _bufferLookup;

        public void OnCreate(ref SystemState state)
        {
            _bufferLookup = state.GetBufferLookup<WaypointBuffer>(true);
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            var timeDeltaTime = SystemAPI.Time.DeltaTime;
            _bufferLookup.Update(ref state);
            
            new MovingJob
            {
                DeltaTime = timeDeltaTime,
                ECB = ecb.AsParallelWriter(),
                BufferLookup = _bufferLookup
            }.ScheduleParallel();
        }
    }
}