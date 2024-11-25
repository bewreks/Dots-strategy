using Components;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public partial class MainCameraInitializeSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<MainCameraTag>();
        }

        protected override void OnUpdate()
        {
            Enabled = false;
            var mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            EntityManager.SetComponentData(mainCameraEntity, new MainCamera
            {
                Camera = Camera.main
            });
        }
    }
}