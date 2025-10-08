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

        private List<GameObject> photosInGallery = new List<GameObject>();
        private float sizePerImage = 0;

        void Start()
        {
            Debug.Log("Gallery subscribing to galleryOpenEvent");
            GameManager.galleryOpenEvent += CreateGalleryList;
            GameManager.galleryCloseEvent += RemoveGalleryList;

            sizePerImage = contentArea.GetComponent<GridLayoutGroup>().cellSize.y + contentArea.GetComponent<GridLayoutGroup>().spacing.y;
        }

        void CreateGalleryList()
        {
            List<Texture2D> Photos = photoImage.LoadAllPhotos();

            foreach (Texture2D photo in Photos)
            {
                var GalleryImage = Instantiate(imagePrefab, contentArea.transform);
                GalleryImage.GetComponent<RawImage>().texture = photo;
                photosInGallery.Add(GalleryImage);
            }

            double YHeight = Mathf.CeilToInt((float)photosInGallery.Count / 2);
            YHeight *= sizePerImage;
            contentArea.GetComponent<RectTransform>().sizeDelta = new Vector2(contentArea.GetComponent<RectTransform>().sizeDelta.x, (float)YHeight + 10);
        }

        void RemoveGalleryList()
        {
            for (int i = photosInGallery.Count - 1; i >= 0; --i)
            {
                var photo = photosInGallery[i];
                photosInGallery.Remove(photosInGallery[i]);
                Destroy(photo);
            }
        }
    }
}