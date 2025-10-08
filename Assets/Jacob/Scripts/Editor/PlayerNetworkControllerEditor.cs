using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlayerNetworkController))]

[AddComponentMenu("Parkour Game/PlayerNetworkControllerEditor")]
public class PlayerNetworkControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        PlayerNetworkController playerNetworkController = (PlayerNetworkController)target;

        if (GUILayout.Button("Connect To Server"))
        {
            playerNetworkController.ConnectToServer();
        }

        if (GUILayout.Button("Disconnect From Server"))
        {
            playerNetworkController.DisconnectServer();
        }
    }
}
