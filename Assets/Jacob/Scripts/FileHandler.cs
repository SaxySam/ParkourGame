using UnityEngine;
using UnityEditor;
using System.IO;

namespace EditiorTools
{
    public class FileHandler
    {
        private static void DeletePhotoFile()
        {
            string filePath = Path.Combine(Application.persistentDataPath, "photos.txt");
            File.Delete(filePath);
        }
    }
}