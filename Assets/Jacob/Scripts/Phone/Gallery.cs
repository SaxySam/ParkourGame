using System.Collections.Generic;
using PhotoCamera;
using UnityEngine;
using UnityEngine.UI;

namespace Phone
{
    [AddComponentMenu("Parkour Game/Gallery")]
    public class Gallery : MonoBehaviour
    {
        public PhotoImage photoImage;
        public GameObject imagePrefab;
        public GameObject contentArea;

        private readonly List<GameObject> _photosInGallery = new List<GameObject>();
        private float _sizePerImage = 0;

        private void Start()
        {
            Debug.Log("Gallery subscribing to galleryOpenEvent");
            GameManager.GalleryOpenEvent += CreateGalleryList;
            GameManager.GalleryCloseEvent += RemoveGalleryList;

            _sizePerImage = contentArea.GetComponent<GridLayoutGroup>().cellSize.y + contentArea.GetComponent<GridLayoutGroup>().spacing.y;
        }

        private void CreateGalleryList()
        {
            List<Texture2D> photos = photoImage.LoadAllPhotos();

            foreach (Texture2D photo in photos)
            {
                GameObject galleryImage = Instantiate(imagePrefab, contentArea.transform);
                galleryImage.GetComponent<RawImage>().texture = photo;
                _photosInGallery.Add(galleryImage);
            }

            double yHeight = Mathf.CeilToInt((float)_photosInGallery.Count / 2);
            yHeight *= _sizePerImage;
            contentArea.GetComponent<RectTransform>().sizeDelta = new Vector2(contentArea.GetComponent<RectTransform>().sizeDelta.x, (float)yHeight + 10);
        }

        private void RemoveGalleryList()
        {
            for (int i = _photosInGallery.Count - 1; i >= 0; --i)
            {
                GameObject photo = _photosInGallery[i];
                _photosInGallery.Remove(_photosInGallery[i]);
                Destroy(photo);
            }
        }
    }
}