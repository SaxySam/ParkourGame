using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Phone;

public class ImagePlacement : MonoBehaviour
{
    public GameObject ImagePrefab;
    public GameObject GhostPrefab;
    public GameObject EnlargedImage;
    public GameObject Gallery;
    public CinemachineCamera firstPersonCamera;
    public PhoneController phoneController;


    private PlayerInput _playerInputComponent;

    private GameObject _ghostImage;

    private Texture _imageTexture;

    private bool _canPlaceImage = false;

    private void OnEnable()
    {
        _playerInputComponent = FindFirstObjectByType<PlayerInput>();
        _playerInputComponent.actions.FindAction("FirstPersonCamera/TakePhoto").performed += PlaceImage;
    }

    private void OnDisable()
    {
        if (_playerInputComponent != null)
        {
            var action = _playerInputComponent.actions.FindAction("FirstPersonCamera/TakePhoto");
            if (action != null) action.performed -= PlaceImage;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_canPlaceImage)
        {
            RaycastHit Hit;
            Ray Ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(Ray, out Hit))
            {
                _ghostImage.transform.position = Hit.point + (Hit.normal * 0.01f);
                _ghostImage.transform.rotation = Quaternion.LookRotation(Hit.normal);
            }
        }
    }

    public void OnPlaceButton()
    {
        _imageTexture = EnlargedImage.GetComponent<RawImage>().texture;
        EnlargedImage.SetActive(false);
        Gallery.SetActive(false);
        firstPersonCamera.Priority = 2;
        Cursor.lockState = CursorLockMode.Locked;

        _ghostImage = Instantiate(GhostPrefab);

        _canPlaceImage = true;
    }

    private void PlaceImage(InputAction.CallbackContext context)
    {
        if (_canPlaceImage)
        {
            GameObject placedImage = Instantiate(ImagePrefab);
            placedImage.transform.position = _ghostImage.transform.position;
            placedImage.transform.rotation = _ghostImage.transform.rotation;

            // placedImage.GetComponent<RawImage>().texture = _imageTexture;
            placedImage.GetComponentInChildren<RawImage>().texture = _imageTexture;

            Destroy(_ghostImage);
            phoneController.ExitPhone();
            _canPlaceImage = false;
        }
    }
}
