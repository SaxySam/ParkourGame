using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace PhotoCamera
{
    
    [AddComponentMenu("Parkour Game/PhotoImage")]
    public class PhotoImage : MonoBehaviour
    {
        private string _filePath;

        private void Awake()
        {
            _filePath = Path.Combine(Application.persistentDataPath, "photos.txt");
        }

        public void SaveNewPhoto(Texture2D newPhoto)
        {
            PhotoList photoList;

            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                photoList = JsonUtility.FromJson<PhotoList>(json);
            }
            else
            {
                photoList = gameObject.AddComponent<PhotoList>();
            }

            byte[] bytes = newPhoto.EncodeToPNG();
            PhotoData newPhotoData = gameObject.AddComponent<PhotoData>();
            photoList.photos.Add(newPhotoData);

            string updatedJson = JsonUtility.ToJson(photoList);
            File.WriteAllText(_filePath, updatedJson);
        }

        public List<Texture2D> LoadAllPhotos()
        {
            List<Texture2D> loadedTextures = new List<Texture2D>();

            if (!File.Exists(_filePath))
            {
                Debug.Log("No photos found");
                return loadedTextures;
            }

            string json = File.ReadAllText(_filePath);
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
