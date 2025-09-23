using UnityEngine;
using Unity.Netcode;

public class PlayerNetworkController : MonoBehaviour
{
    public void ConnectToServer()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.StartClient();
            Debug.Log("Server Started");
        }
        else
        {
            Debug.LogError("Could not find Network Manager Component");
        }
    }

    public void DisconnectServer()
    {
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsClient)
        {
            NetworkManager.Singleton.Shutdown();
            Debug.Log("Server Stopped");
        }
        else
        {
            Debug.LogError("Could not find Network Manager Component");
        }
    }
}
