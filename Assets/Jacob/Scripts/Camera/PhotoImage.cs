using UnityEngine;
using System.IO;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using System.Collections.Generic;

namespace PhotoCamera
{
    public class PhotoImage : MonoBehaviour
    {
        private string filePath;

        private void Awake()
        {
            filePath = Path.Combine(Application.persistentDataPath, "photos.txt");
        }

        public void SaveNewPhoto(Texture2D newPhoto)
        {
            PhotoList photoList;

            if (File.Exists(filePath))
            {
                string json = File.ReadAllText(filePath);
                photoList = JsonUtility.FromJson<PhotoList>(json);
            }
            else
            {
                photoList = new PhotoList();
            }

            byte[] bytes = newPhoto.EncodeToPNG();
            PhotoData newPhotoData = new PhotoData { pngData = bytes };
            photoList.photos.Add(newPhotoData);

            string updatedJson = JsonUtility.ToJson(photoList);
            File.WriteAllText(filePath, updatedJson);
        }

        public List<Texture2D> LoadAllPhotos()
        {
            List<Texture2D> loadedTextures = new List<Texture2D>();

            if (!File.Exists(filePath))
            {
                Debug.Log("No photos found");
                return loadedTextures;
            }

            string json = File.ReadAllText(filePath);
            PhotoList photoList = JsonUtility.FromJson<PhotoList>(json);

            if (photoList == null || photoList.photos == null)
            {
                Debug.LogError("Failed to deserialize texture list.");
                return loadedTextures;
            }

            foreach (PhotoData data in photoList.photos)
            {
                Texture2D newTexture = new Texture2D(2, 2);
                bool success = newTexture.LoadImage(data.pngData);

                if (success)
                {
                    loadedTextures.Add(newTexture);
                }
                else
                {
                    Debug.LogError("Failed to load image from data.");
                }
            }

            return loadedTextures;
        }
    }

}
