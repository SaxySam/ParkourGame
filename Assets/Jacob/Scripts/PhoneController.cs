using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneController : MonoBehaviour
{
    public InputActionReference pauseButton;
    public InputActionReference escapeAction;
    public GameObject phonePanel;
    public CinemachineCamera firstPersonCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        phonePanel.SetActive(false);
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
        firstPersonCamera.Priority = 2;
    }


}
