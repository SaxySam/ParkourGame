using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;
using Unity.Services.Lobbies.Models;
using PhotoCamera;
using SDK;
using UnityEditor.Localization.Plugins.XLIFF.V12;

namespace Phone
{
    public class PhoneController : MonoBehaviour
    {
        public GameObject phonePanel;
        public GameObject galleryPanel;
        public GameObject EnlargedPhoto;

        public CinemachineCamera firstPersonCamera;
        private PlayerInput playerInputComponent;
        private FirstPersonMovement firstPersonMovement;
        private SamCharacterController samCharacterController;

        void OnEnable()
        {
            playerInputComponent = FindFirstObjectByType<PlayerInput>();
            GameManager.phoneOpenEvent += OpenPhone;
            GameManager.galleryButtonPressedEvent += EnlargePhoto;
            playerInputComponent.actions["Exit"].performed += ExitPhone;
        }

        void OnDisable()
        {
            GameManager.phoneOpenEvent -= OpenPhone;
            GameManager.galleryButtonPressedEvent -= EnlargePhoto;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            firstPersonMovement = FindFirstObjectByType<FirstPersonMovement>();
            samCharacterController = FindFirstObjectByType<SamCharacterController>();

            firstPersonMovement.enabled = false;
            phonePanel.SetActive(false);
            galleryPanel.SetActive(false);
            EnlargedPhoto.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ExitPhone(InputAction.CallbackContext context)
        {
            if (galleryPanel.activeSelf)
            {
                GameManager.galleryCloseEvent();
                galleryPanel.SetActive(false);
            }
            EnlargedPhoto.SetActive(false);
            firstPersonCamera.Priority = 0;
            firstPersonMovement.enabled = false;
            samCharacterController.enabled = true;
        }

        public void OpenPhone()
        {
            firstPersonMovement.enabled = true;
            samCharacterController.enabled = false;
            phonePanel.SetActive(!phonePanel.activeSelf);
            Cursor.lockState = CursorLockMode.None;
        }

        public void OpenCamera()
        {
            phonePanel.SetActive(false);
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