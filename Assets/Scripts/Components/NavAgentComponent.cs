using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct NavAgentComponent : IComponentData
    {
        public int CurrentWaypoint;
    }
    
    public struct NavAgentActivated : IComponentData, IEnableableComponent { }

    public struct WaypointBuffer : IBufferElementData
    {
        public float3 WayPoint;
    }
}