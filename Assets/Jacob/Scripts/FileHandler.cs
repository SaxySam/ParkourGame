using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditiorTools
{
    public class FileHandler
    {
        [MenuItem("Tools/Delete Photo Data")]
        private static void DeletePhotoFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "photos.txt");
            File.Delete(filePath);
        }
    }
}