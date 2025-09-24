using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

namespace Phone
{
    public class PhoneController : MonoBehaviour
    {
        public InputActionReference pauseButton;
        public InputActionReference escapeAction;

        public GameObject phonePanel;
        public GameObject galleryPanel;
        public GameObject photoCamera;
        public GameObject EnlargedPhoto;

        public CinemachineCamera firstPersonCamera;


        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            GameManager.galleryButtonPressedEvent += EnlargePhoto;
            phonePanel.SetActive(false);
            galleryPanel.SetActive(false);
            photoCamera.SetActive(false);
            EnlargedPhoto.SetActive(false);
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
                    GameManager.galleryCloseEvent();
                    galleryPanel.SetActive(false);
                }
                EnlargedPhoto.SetActive(false);
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
            galleryPanel.SetActive(true);
            phonePanel.SetActive(false);
            Debug.Log("Invoking galleryOpenEvent");
            GameManager.galleryOpenEvent();
        }

        void EnlargePhoto(Texture texture)
        {
            EnlargedPhoto.SetActive(true);
            EnlargedPhoto.GetComponent<RawImage>().texture = texture;
        }
    }
}