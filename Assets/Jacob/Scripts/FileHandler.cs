using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditiorTools
{
    [AddComponentMenu("Parkour Game/FileHandler")]
    public class FileHandler
    {
        [MenuItem("Tools/Delete Photo Data")]
        static void DeletePhotoFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "photos.txt");
            File.Delete(filePath);
        }
    }
}