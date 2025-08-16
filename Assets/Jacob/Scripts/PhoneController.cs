using UnityEngine;
using UnityEngine.InputSystem;

public class PhoneController : MonoBehaviour
{
    public InputActionReference pauseButton;
    public GameObject phonePanel;

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
    }

    public void OpenPhone()
    {
        phonePanel.SetActive(!phonePanel.activeSelf);
    }
}
