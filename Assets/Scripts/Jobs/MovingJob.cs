using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Jobs
{
    [WithAll(typeof(MovingActivated))]
    public partial struct MovingJob : IJobEntity
    {
        public float DeltaTime;
        
        public EntityCommandBuffer.ParallelWriter ECB;
        [ReadOnly] public BufferLookup<WaypointBuffer> BufferLookup;

        private void Execute(Entity entity,
                             [EntityIndexInQuery] int index,
                             ref NavAgentComponent agent, 
                             in MoveSpeed moveSpeed, 
                             in RotationSpeed rotationSpeed,
                             ref LocalTransform transform)
        {
            var waypointBuffer = BufferLookup[entity];
            if (waypointBuffer.Length == 0)
            {
                ECB.SetComponentEnabled<MovingActivated>(index, entity, false);
                return;
            }
                
            if (math.distance(transform.Position, waypointBuffer[agent.CurrentWaypoint].WayPoint) < 0.4f)
            {
                if (agent.CurrentWaypoint + 1 < waypointBuffer.Length)
                {
                    agent.CurrentWaypoint += 1;
                }
                else
                {
                    transform.Position = waypointBuffer[agent.CurrentWaypoint].WayPoint;
                    ECB.SetComponentEnabled<MovingActivated>(index, entity, false);
                    return;
                }
            }
                
            var direction = waypointBuffer[agent.CurrentWaypoint].WayPoint - transform.Position;
            var angle = math.degrees(math.atan2(direction.z, direction.x));

            transform.Rotation = math.slerp(
                transform.Rotation,
                quaternion.Euler(new float3(0, angle, 0)),
                DeltaTime * rotationSpeed.Value);

            transform.Position += math.normalize(direction) * (DeltaTime * moveSpeed.Value);
        }
    }
}