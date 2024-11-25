using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class MainCameraAuthoring : MonoBehaviour
    {
        public GameObject target;
        
        private class Baker : Baker<MainCameraAuthoring>
        {
            public override void Bake(MainCameraAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponentObject(entity, new MainCamera());
                AddComponent(entity, new MainCameraTag());
                AddComponent(entity, new Sphere { Value = GetEntity(authoring.target, TransformUsageFlags.Dynamic) });
            }
        }
    }

    public struct Sphere : IComponentData
    {
        public Entity Value;
    }
}