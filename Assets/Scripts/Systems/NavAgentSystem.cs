using System;
using Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.Experimental.AI;
using Utils;

namespace Systems
{
    [UpdateAfter(typeof(UserInputSystem))]
    public partial struct NavAgentSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
        }

        [Obsolete("Obsolete")]
        public void OnUpdate(ref SystemState state)
        {
            var ecb = new EntityCommandBuffer(Allocator.Temp);//  SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
            foreach (var (agent, 
                         transform, 
                         target,
                         entity) 
                     in SystemAPI.Query<RefRW<NavAgentComponent>, 
                                        RefRO<LocalTransform>,
                                        RefRO<MoveTarget>>()
                         .WithAll<NavAgentActivated>()
                         .WithEntityAccess())
            {
                ecb.SetComponentEnabled<NavAgentActivated>(entity, false);
                
                agent.ValueRW.CurrentWaypoint = 0;
                
                var waypointBuffer = state.EntityManager.GetBuffer<WaypointBuffer>(entity);
                var query = new NavMeshQuery(NavMeshWorld.GetDefaultWorld(), Allocator.Temp, 1000);
                var fromPosition = transform.ValueRO.Position;
                var toPosition = target.ValueRO.Value;
                var extents = new float3(1, 1, 1);
                
                var fromLocation = query.MapLocation(fromPosition, extents, 0);
                var toLocation = query.MapLocation(toPosition, extents, 0);
                
                PathQueryStatus status;
                PathQueryStatus returningStatus;
                const int maxPathSize = 100;
                
                if (query.IsValid(fromLocation) && query.IsValid(toLocation))
                {
                    status = query.BeginFindPath(fromLocation, toLocation);

                    while (status == PathQueryStatus.InProgress)
                    {
                        status = query.UpdateFindPath(100, out var iterationsPerformed);
                    }

                    if (status == PathQueryStatus.Success)
                    {
                        status = query.EndFindPath(out var pathSize);

                        var result = new NativeArray<NavMeshLocation>(pathSize + 1, Allocator.Temp);
                        var straightPathFlag = new NativeArray<StraightPathFlags>(maxPathSize, Allocator.Temp);
                        var vertexSide = new NativeArray<float>(maxPathSize, Allocator.Temp);
                        var polygonIds = new NativeArray<PolygonId>(pathSize + 1, Allocator.Temp);
                        var straightPathCount = 0;

                        query.GetPathResult(polygonIds);

                        returningStatus = PathUtils.FindStraightPath
                            (
                            query,
                            fromPosition,
                            toPosition,
                            polygonIds,
                            pathSize,
                            ref result,
                            ref straightPathFlag,
                            ref vertexSide,
                            ref straightPathCount,
                            maxPathSize
                            );

                        if(returningStatus == PathQueryStatus.Success)
                        {
                            waypointBuffer.Clear();

                            foreach (var location in result)
                            {
                                if (location.position != Vector3.zero)
                                {
                                    waypointBuffer.Add(new WaypointBuffer { WayPoint = location.position });
                                }
                            }
                        }
                        straightPathFlag.Dispose();
                        polygonIds.Dispose();
                        vertexSide.Dispose();
                    }
                }
                query.Dispose();
                ecb.SetComponentEnabled<MovingActivated>(entity, true);
            }
            ecb.Playback(state.EntityManager);
        }
    }
}