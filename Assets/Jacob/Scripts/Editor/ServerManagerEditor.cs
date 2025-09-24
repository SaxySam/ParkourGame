using UnityEngine;
using UnityEditor;
using Networking;

[CustomEditor(typeof(ServerManager))]

public class ServerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        ServerManager serverManager = (ServerManager)target;

        if (GUILayout.Button("Display Clients"))
        {
            serverManager.DisplayClients();
        }
    }
}
