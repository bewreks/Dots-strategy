using Components;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Systems
{
    [UpdateAfter(typeof(NavAgentSystem))]
    public partial struct MovingSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        public void OnUpdate(ref SystemState state)
        {
            var ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            
            var timeDeltaTime = SystemAPI.Time.DeltaTime;
            foreach (var (agent,
                         moveSpeed,
                         rotationSpeed,
                         transform,
                         entity) 
                     in SystemAPI.Query<RefRW<NavAgentComponent>,
                             RefRO<MoveSpeed>,
                             RefRO<RotationSpeed>,
                             RefRW<LocalTransform>>()
                         .WithAll<MovingActivated>()
                         .WithEntityAccess())
            {
                var waypointBuffer = state.EntityManager.GetBuffer<WaypointBuffer>(entity);

                if (waypointBuffer.Length == 0)
                {
                    ecb.SetComponentEnabled<MovingActivated>(entity, false);
                    continue;
                }
                
                if (math.distance(transform.ValueRO.Position, waypointBuffer[agent.ValueRO.CurrentWaypoint].WayPoint) < 0.4f)
                {
                    if (agent.ValueRO.CurrentWaypoint + 1 < waypointBuffer.Length)
                    {
                        agent.ValueRW.CurrentWaypoint += 1;
                    }
                    else
                    {
                        transform.ValueRW.Position = waypointBuffer[agent.ValueRO.CurrentWaypoint].WayPoint;
                        ecb.SetComponentEnabled<MovingActivated>(entity, false);
                        waypointBuffer.Clear();
                        return;
                    }
                }
                
                var direction = waypointBuffer[agent.ValueRO.CurrentWaypoint].WayPoint - transform.ValueRO.Position;
                var angle = math.degrees(math.atan2(direction.z, direction.x));

                transform.ValueRW.Rotation = math.slerp(
                    transform.ValueRW.Rotation,
                    quaternion.Euler(new float3(0, angle, 0)),
                    timeDeltaTime * rotationSpeed.ValueRO.Value);

                transform.ValueRW.Position += math.normalize(direction) * (timeDeltaTime * moveSpeed.ValueRO.Value);
            }
        }
    }
}