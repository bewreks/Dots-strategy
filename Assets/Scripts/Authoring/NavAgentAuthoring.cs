using Components;
using Unity.Entities;
using UnityEngine;

namespace Authoring
{
    public class NavAgentAuthoring : MonoBehaviour
    {
        private class Baker : Baker<NavAgentAuthoring>
        {
            public override void Bake(NavAgentAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);

                AddComponent<NavAgentComponent>(entity);
                AddComponent<NavAgentActivated>(entity);
                AddBuffer<WaypointBuffer>(entity);
                SetComponentEnabled<NavAgentActivated>(entity, false);
            }
        }
    }
}