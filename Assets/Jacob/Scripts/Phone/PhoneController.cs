using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using PhotoCamera;
using SDK;
using UnityEngine.Serialization;

namespace Phone
{
    [AddComponentMenu("Parkour Game/PhoneController")]
    public class PhoneController : MonoBehaviour
    {
        public GameObject phonePanel;
        public GameObject galleryPanel;
        [FormerlySerializedAs("EnlargedPhoto")] public GameObject enlargedPhoto;

        public CinemachineCamera firstPersonCamera;
        private PlayerInput _playerInputComponent;
        private FirstPersonMovement _firstPersonMovement;
        private SamCharacterController _samCharacterController;

        private void OnEnable()
        {
            _playerInputComponent = FindFirstObjectByType<PlayerInput>();
            GameManager.PhoneOpenEvent += EnableDisablePhone;
            GameManager.GalleryButtonPressedEvent += EnlargePhoto;
            _playerInputComponent.actions.FindAction("FirstPersonCamera/Exit").performed += ExitPhone;
        }

        private void OnDisable()
        {
            GameManager.PhoneOpenEvent -= EnableDisablePhone;
            GameManager.GalleryButtonPressedEvent -= EnlargePhoto;
#if !UNITY_EDITOR
                _playerInputComponent.actions.FindAction("FirstPersonCamera/Exit").performed += ExitPhone;
#endif
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            _firstPersonMovement = FindFirstObjectByType<FirstPersonMovement>();
            _samCharacterController = FindFirstObjectByType<SamCharacterController>();

            _firstPersonMovement.enabled = false;
            phonePanel.SetActive(false);
            galleryPanel.SetActive(false);
            enlargedPhoto.SetActive(false);
        }


        public void OpenPhone()
        {
            _firstPersonMovement.enabled = true;
            _samCharacterController.enabled = false;
            phonePanel.SetActive(true);
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
            GameManager.GalleryOpenEvent();
        }

        public void EnableDisablePhone()
        {
            if (phonePanel.activeSelf)
            {
                ExitPhone();
                return;
            }
            OpenPhone();
        }

        public void EnlargePhoto(Texture texture)
        {
            enlargedPhoto.SetActive(true);
            enlargedPhoto.GetComponent<RawImage>().texture = texture;
        }

        public void ExitPhone()
        {
            phonePanel.SetActive(false);
            if (galleryPanel.activeSelf)
            {
                GameManager.GalleryCloseEvent();
                galleryPanel.SetActive(false);
            }
            enlargedPhoto.SetActive(false);
            firstPersonCamera.Priority = 0;
            _firstPersonMovement.enabled = false;
            _samCharacterController.enabled = true;
        }

        /// <summary>
        /// Overridden Exit Phone Method to allow for Input Delegate calling with Exit action
        /// </summary>
        /// <param name="context">The input action context. Unused.</param>
        public void ExitPhone(InputAction.CallbackContext context)
        {
            ExitPhone();
        }
    }
}