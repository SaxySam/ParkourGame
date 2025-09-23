using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Phone
{
    public class PhoneController : MonoBehaviour
    {
        public static GameManager.OnGalleryOpen galleryOpenEvent;
        public static GameManager.OnGalleryClose galleryCloseEvent;

        public InputActionReference pauseButton;
        public InputActionReference escapeAction;
        public GameObject phonePanel;
        public GameObject galleryPanel;
        public GameObject photoCamera;
        public CinemachineCamera firstPersonCamera;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            phonePanel.SetActive(false);
            galleryPanel.SetActive(false);
            photoCamera.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {
            if (pauseButton.action.triggered)
            {
                OpenPhone();
            }

            if (escapeAction.action.IsPressed())
            {
                photoCamera.SetActive(false);
                if (galleryPanel.activeSelf)
                {
                    galleryCloseEvent();
                    galleryPanel.SetActive(false);
                }
                firstPersonCamera.Priority = 0;
            }
        }

        public void OpenPhone()
        {
            phonePanel.SetActive(!phonePanel.activeSelf);
            Cursor.lockState = CursorLockMode.None;
        }

        public void OpenCamera()
        {
            phonePanel.SetActive(false);
            photoCamera.SetActive(true);
            firstPersonCamera.Priority = 2;
        }

        public void OpenGallery()
        {
            galleryOpenEvent();
            phonePanel.SetActive(false);
            galleryPanel.SetActive(true);
        }

    }
}