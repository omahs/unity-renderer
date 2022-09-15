using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DCL;
using rpc_csharp;
using UnityEngine;

namespace RPC.Services
{
    public static class TeleportServiceImpl
    {
        private static readonly UniTask<Teleport.Types.FromKernel.Types.TeleportResponse> defaultTeleportResponse = UniTask.FromResult(new Teleport.Types.FromKernel.Types.TeleportResponse());
        private static readonly UniTask<Teleport.Types.FromKernel.Types.RequestTeleportResponse> defaultRequestTeleportResponse = UniTask.FromResult(new Teleport.Types.FromKernel.Types.RequestTeleportResponse());

        public static void RegisterService(RpcServerPort<RPCContext> port)
        {
            TeleportService<RPCContext>.RegisterService(
                port,
                teleport: Teleport,
                requestTeleport: RequestTeleport,
                onMessage: OnMessage
            );
        }

        private static IEnumerator<Teleport.Types.FromRenderer> OnMessage(Teleport.Types.FromRenderer.Types.StreamRequest request, RPCContext context)
        {
            while (true)
            {
                if (context.teleportContext.queueMessages.Count > 0)
                {
                    Teleport.Types.FromRenderer message = context.teleportContext.queueMessages.Dequeue();
                    yield return message;
                }
                else
                {
                    yield return null;
                }
            }
        }
        private static UniTask<Teleport.Types.FromKernel.Types.RequestTeleportResponse> RequestTeleport(Teleport.Types.FromKernel.Types.RequestTeleport request, RPCContext context, CancellationToken ct)
        {
            HUDController.i.teleportHud?.RequestTeleport(request.Destination);
            return defaultRequestTeleportResponse;
        }
        private static UniTask<Teleport.Types.FromKernel.Types.TeleportResponse> Teleport(Teleport.Types.FromKernel.Types.Teleport request, RPCContext context, CancellationToken ct)
        {
            var position = new Vector3() { x = request.Position.X, y = request.Position.Y, z = request.Position.Z };
            DCLCharacterController.i.Teleport(position);

            Vector3? cameraTarget = null;
            if (request.CameraTarget != null) cameraTarget = new Vector3() { x = request.CameraTarget.X, y = request.CameraTarget.Y, z = request.CameraTarget.Z };

            if (cameraTarget != null || request.RotateIfTargetIsNotSet)
            {
                SceneReferences.i.cameraController.SetRotation(position.x, position.y, position.z, cameraTarget);
            }

            return defaultTeleportResponse;
        }
    }
}