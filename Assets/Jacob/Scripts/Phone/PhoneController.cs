using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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
            GameManager.phoneOpenEvent += EnableDisablePhone;
            GameManager.galleryButtonPressedEvent += EnlargePhoto;
            playerInputComponent.actions.FindAction("FirstPersonCamera/Exit").performed += ExitPhone;
        }

        void OnDisable()
        {
            GameManager.phoneOpenEvent -= EnableDisablePhone;
            GameManager.galleryButtonPressedEvent -= EnlargePhoto;
#if !UNITY_EDITOR
                playerInputComponent.actions.FindAction("FirstPersonCamera/Exit").performed += ExitPhone;
#endif
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


        public void OpenPhone()
        {
            firstPersonMovement.enabled = true;
            samCharacterController.enabled = false;
            phonePanel.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
        }

        public void OpenCamera()
        {
            phonePanel.SetActive(false);
            firstPersonCamera.Priority = 2;
            Cursor.lockState = CursorLockMode.Locked;
        }

        public void OpenGallery()
        {
            galleryPanel.SetActive(true);
            phonePanel.SetActive(false);
            Debug.Log("Invoking galleryOpenEvent");
            GameManager.galleryOpenEvent();
        }

        void EnableDisablePhone()
        {
            if (phonePanel.activeSelf)
            {
                ExitPhone();
                return;
            }
            OpenPhone();
        }

        void EnlargePhoto(Texture texture)
        {
            EnlargedPhoto.SetActive(true);
            EnlargedPhoto.GetComponent<RawImage>().texture = texture;
        }

        public void ExitPhone()
        {
            phonePanel.SetActive(false);
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

        /// <summary>
        /// Overridden Exit Phone Method to allow for Input Delegate calling with Exit action
        /// </summary>
        /// <param name="context">The input action context. Unused.</param>
        void ExitPhone(InputAction.CallbackContext context)
        {
            ExitPhone();
        }
    }
}