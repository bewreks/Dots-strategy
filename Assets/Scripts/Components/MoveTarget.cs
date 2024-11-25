using Unity.Entities;
using Unity.Mathematics;

namespace Components
{
    public struct MoveTarget : IComponentData
    {
        public float3 Value;
    }
    
    public struct MoveSpeed : IComponentData
    {
        public float Value;
    }

    public struct RotationSpeed : IComponentData
    {
        public float Value;
    }
    
    public struct MovingActivated : IComponentData, IEnableableComponent { }
}