using UnityEngine;
using UnityEditor;
using System.IO;

[AddComponentMenu("Parkour Game/GameManager")]
public class FileDeletion : MonoBehaviour
{
    public void DeletePhotoFile()
    {
        string filePath = Path.Combine(Application.persistentDataPath, "photos.txt");
        File.Delete(filePath);
    }

    public void Print()
    {
        Debug.Log(Application.persistentDataPath);
    }
}
