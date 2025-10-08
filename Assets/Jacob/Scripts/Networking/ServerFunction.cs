using Unity.Netcode;
using UnityEngine;

[AddComponentMenu("Parkour Game/ServerFunction")]
public class ServerFunction : NetworkBehaviour
{
    [Rpc(SendTo.Server)]
    public void AddPhotoToServerListServerRPC(/*Texture2D photo*/)
    {

    }

    [Rpc(SendTo.Server)]
    public void RequestPhotosServerRPC(RpcParams rpcParams = default)
    {

    }

    [Rpc(SendTo.SpecifiedInParams)]
    public void SetSharedPhotoListClientRPC(RpcParams rpcParams)
    {

    }
}
