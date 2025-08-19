using UnityEngine;
using Unity.Netcode;

namespace Networking
{
    public class ServerManager : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            if (NetworkManager.Singleton != null)
            {
                NetworkManager.Singleton.StartServer();
                Debug.Log("Server Started");
            }
            else
            {
                Debug.LogError("Could not find Network Manager Component");
            }
        }
    }
}