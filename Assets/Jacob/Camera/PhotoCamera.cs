using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PhotoCamera
{
    public class PhotoCamera : MonoBehaviour
    {
        [SerializeField] private RenderTexture sourceTexture;
        public InputActionReference cycleAction;
        public InputActionReference takePhotoAction;
        public PhotoImage photoImage;

        // Display on screen
        public RawImage rawImage;

        void TakePhoto()
        {
            // Takes the camera view and saves it to a Texture2D
            Texture2D image = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = sourceTexture;
            image.ReadPixels(new Rect(0, 0, sourceTexture.width, sourceTexture.height), 0, 0);
            image.Apply();
            RenderTexture.active = null;

            // Saves image to file
            photoImage.SaveNewPhoto(image);
        }

        void Update()
        {
            if (takePhotoAction.action.IsPressed())
            {
                TakePhoto();
            }

            if (cycleAction.action.IsPressed())
            {
                List<Texture2D> images = photoImage.LoadAllPhotos();
                StartCoroutine(CyclePhotos(images));
            }

        }

        IEnumerator CyclePhotos(List<Texture2D> images)
        {
            foreach (Texture2D image in images)
            {
                rawImage.texture = image;
                yield return new WaitForSeconds(7);
            }
        }
    }
}