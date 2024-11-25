using Unity.Entities;
using UnityEngine;

namespace Components
{
    public class MainCamera : IComponentData
    {
        public Camera Camera;
    }
    
    public struct MainCameraTag : IComponentData { }
}