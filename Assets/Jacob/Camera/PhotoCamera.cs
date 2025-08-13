using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace PhotoCamera
{

    [RequireComponent(typeof(Camera))]
    public class PhotoCamera : MonoBehaviour
    {
        [SerializeField] private RenderTexture sourceTexture;
        public InputActionReference pauseAction;
        // public InputAction leftClick;

        public RawImage rawImage;

        void TakePhoto()
        {
            // Takes the camera view and saves it to a Texture2D
            Texture2D image = new Texture2D(sourceTexture.width, sourceTexture.height, TextureFormat.RGBA32, false);
            RenderTexture.active = sourceTexture;
            image.ReadPixels(new Rect(0, 0, sourceTexture.width, sourceTexture.height), 0, 0);
            image.Apply();
            RenderTexture.active = null;

            // Outputs texture onto UI image
            rawImage.texture = image;
        }

        void Update()
        {
            if (pauseAction.action.IsPressed())
            {
                TakePhoto();
            }
        }
    }
}