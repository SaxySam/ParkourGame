using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

namespace Networking
{
    [AddComponentMenu("Parkour Game/ServerManager")]
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

        public void DisplayClients()
        {
            if (NetworkManager.Singleton.IsServer)
            {
                foreach (NetworkClient client in NetworkManager.Singleton.ConnectedClientsList)
                {
                    Debug.Log("Client ID: " + client.ClientId);
                }
            }
        }
    }
}