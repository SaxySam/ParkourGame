using UnityEngine;


namespace PhotoCamera
{
    public class PhotoCamera : MonoBehaviour
    {
        [SerializeField] private RenderTexture sourceTexture;
        public PhotoImage photoImage;
        [SerializeField] private CameraFlash cameraFlash;

        public void TakePhoto()
        {
            Debug.Log("Taking Photo");

            cameraFlash.FlashCamera();
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

        }
    }
}