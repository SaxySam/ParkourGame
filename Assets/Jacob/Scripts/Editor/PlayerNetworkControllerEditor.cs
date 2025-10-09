using UnityEngine;
using UnityEditor;

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
