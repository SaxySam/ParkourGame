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
        public GameObject photoCamera;
        public InputActionReference cycleAction;
        public InputActionReference takePhotoAction;
        public PhotoImage photoImage;

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
            if (takePhotoAction.action.IsPressed() && photoCamera.activeSelf)
            {
                TakePhoto();
            }

            if (cycleAction.action.IsPressed())
            {
                List<Texture2D> images = photoImage.LoadAllPhotos();
            }
        }
    }
}