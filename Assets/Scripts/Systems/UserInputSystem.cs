using Authoring;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class UserInputSystem : SystemBase
    {
        private InputSystem_Actions _inputSystemActions;

        protected override void OnCreate()
        {
            _inputSystemActions = new InputSystem_Actions();
            _inputSystemActions.Player.Enable();
            
            RequireForUpdate<MainCameraTag>();
        }

        protected override void OnStartRunning()
        {
            _inputSystemActions.Player.Move.performed += OnMove;
        }

        protected override void OnStopRunning()
        {
            _inputSystemActions.Player.Move.performed += OnMove;
        }
        
        private void OnMove(InputAction.CallbackContext context)
        {
                
            var mainCameraEntity = SystemAPI.GetSingletonEntity<MainCameraTag>();
            var camera = EntityManager.GetComponentObject<MainCamera>(mainCameraEntity).Camera;

            var ray = camera.ScreenPointToRay(_inputSystemActions.Player.PointerPosition.ReadValue<Vector2>());
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (!plane.Raycast(ray, out var distance)) return;
            
            var ecb = new EntityCommandBuffer(Allocator.Temp);
            foreach (var (target, 
                         entity) 
                     in SystemAPI.Query<RefRW<MoveTarget>>()
                         .WithAll<UnitTag>()
                         .WithEntityAccess())
            {
                target.ValueRW.Value = ray.GetPoint(distance);
                ecb.SetComponentEnabled<NavAgentActivated>(entity, true);
            }
                
            var value = EntityManager.GetComponentData<Sphere>(mainCameraEntity).Value;
            var transform = EntityManager.GetComponentData<LocalTransform>(value);
            transform.Position = ray.GetPoint(distance);
            ecb.SetComponent(value, transform);
            ecb.Playback(EntityManager);
        }

        protected override void OnUpdate()
        {
            
        }
    }
}