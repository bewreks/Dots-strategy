using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class UnitAuthoring : MonoBehaviour
    {
        private class Baker : Baker<UnitAuthoring>
        {
            public override void Bake(UnitAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent<UnitTag>(entity);
            }
        }
    }
}