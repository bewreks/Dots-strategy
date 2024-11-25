using Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authoring
{
    public class MovingAuthoring : MonoBehaviour
    {
        public float movingSpeed;
        public float rotationSpeed;
        
        private class Baker : Baker<MovingAuthoring>
        {
            public override void Bake(MovingAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<MoveTarget>(entity);
                AddComponent<MovingActivated>(entity);
                AddComponent(entity, new MoveSpeed
                {
                    Value = authoring.movingSpeed
                });
                AddComponent(entity, new RotationSpeed
                {
                    Value = authoring.rotationSpeed
                });
                SetComponentEnabled<MovingActivated>(entity, false);
            }
        }
    }
}