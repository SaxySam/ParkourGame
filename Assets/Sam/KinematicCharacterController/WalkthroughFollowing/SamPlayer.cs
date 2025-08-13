using System.Linq;
using KinematicCharacterController.Examples;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using KinematicCharacterController;

namespace SDK
{

    [System.Serializable]
    public struct FMouseInputActions
    {
        public InputActionReference mouseLookAction;
        public InputActionReference mouseZoomAction;
        public InputActionReference mouseLeftClickAction;
        public InputActionReference mouseMiddleClickAction;
        public InputActionReference mouseRightClickAction;
        public Vector2 mouseSensitivity;
        public bool useRawInput;

        // Constructor
        public FMouseInputActions(InputActionReference mouseLookAction, InputActionReference mouseZoomAction, InputActionReference mouseLeftClickAction, InputActionReference mouseMiddleClickAction, InputActionReference mouseRightClickAction, Vector2 mouseSensitivity, bool useRawInput)
        {
            this.mouseLookAction = mouseLookAction;
            this.mouseZoomAction = mouseZoomAction;
            this.mouseLeftClickAction = mouseLeftClickAction;
            this.mouseMiddleClickAction = mouseMiddleClickAction;
            this.mouseRightClickAction = mouseRightClickAction;
            this.mouseSensitivity = mouseSensitivity = new Vector2(200, 200);
            this.useRawInput = useRawInput = true;
        }
    }

    [System.Serializable]
    public struct FPlayerInputActions
    {
        public InputActionReference moveInputAction;
        public InputActionReference jumpInputAction;
        public InputActionReference sprintInputAction;
        public InputActionReference crouchInputAction;

        // Constructor
        public FPlayerInputActions(InputActionReference moveInputAction, InputActionReference jumpInputAction, InputActionReference sprintInputAction, InputActionReference crouchInputAction, Quaternion cameraRotation)
        {
            this.moveInputAction = moveInputAction;
            this.jumpInputAction = jumpInputAction;
            this.sprintInputAction = sprintInputAction;
            this.crouchInputAction = crouchInputAction;
        }
    }

    public class SamPlayer : MonoBehaviour
    {
        public FMouseInputActions mouseInputs;
        public FPlayerInputActions playerInputActions;

        public ExampleCharacterCamera orbitCamera;
        public SamCharacterController character;
        public Vector3 inputVector = Vector3.zero;

        private void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;

            // Tell camera to follow transform
            orbitCamera.SetFollowTransform(character.cameraFollowPoint);

            // Ignore the character's collider(s) for camera obstruction checks
            orbitCamera.IgnoredColliders.Clear();
            orbitCamera.IgnoredColliders.AddRange(character.GetComponentsInChildren<Collider>());
        }

        private void Update()
        {
            if (mouseInputs.mouseLeftClickAction.action.triggered && mouseInputs.mouseLeftClickAction.action.ReadValue<float>() > 0)
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
        }

        private void HandleCameraInput()
        {
            // Create the look input vector for the camera
            float mouseLookAxisUp = mouseInputs.mouseLookAction.action.ReadValue<Vector2>().y * (mouseInputs.mouseSensitivity.y / 25);
            float mouseLookAxisRight = mouseInputs.mouseLookAction.action.ReadValue<Vector2>().x * (mouseInputs.mouseSensitivity.x / 25);
            Vector3 _lookInputVector = mouseInputs.useRawInput ? new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f) : new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f) * Time.deltaTime;

            // Prevent moving the camera while the cursor isn't locked
            if (Cursor.lockState != CursorLockMode.Locked)
            {
                _lookInputVector = Vector3.zero;
            }

            // Input for zooming the camera (disabled in WebGL because it can cause problems)
            float scrollInput = -mouseInputs.mouseZoomAction.action.ReadValue<Vector2>().y;
#if UNITY_WEBGL
        scrollInput = 0f;
#endif

            // Apply inputs to the camera
            orbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector);

            // Handle toggling zoom level
            if (mouseInputs.mouseRightClickAction.action.triggered && mouseInputs.mouseRightClickAction.action.ReadValue<float>() > 0)
            {
                orbitCamera.TargetDistance = (orbitCamera.TargetDistance == 0f) ? orbitCamera.DefaultDistance : 0f;
            }
        }

        private void HandleCharacterInput()
        {
            // Build the CharacterInputs struct
            FPlayerInputs characterInputs = new()
            {
                moveAxisForward = playerInputActions.moveInputAction.action.ReadValue<Vector2>().y,

                moveAxisRight = playerInputActions.moveInputAction.action.ReadValue<Vector2>().x,

                cameraRotation = orbitCamera.Transform.rotation,

                jumpDown = playerInputActions.jumpInputAction.action.triggered && playerInputActions.jumpInputAction.action.ReadValue<float>() > 0,

                //TODO Needs refactoring for New Input System

                // crouchDown = playerInputActions.crouchInputAction.action.triggered && playerInputActions.crouchInputAction.action.ReadValue<float>() > 0,

                // crouchUp = playerInputActions.crouchInputAction.action.triggered && playerInputActions.crouchInputAction.action.ReadValue<float>() == 0,

                crouchDown = Input.GetKeyDown(KeyCode.C),
                
                crouchUp = Input.GetKeyUp(KeyCode.C),

            };

            // Apply inputs to character
            character.SetInputs(ref characterInputs);

            // Apply impulse
            if (Input.GetKeyDown(KeyCode.Q))
            {
                character.motor.ForceUnground(0.1f);
                character.AddVelocity(Vector3.one * 10f);
            }
        }
    }

    /*
        public static bool GetKey/GetButton(this InputAction action) => action.ReadValue<float>() > 0;
        public static bool GetKeyDown/GetButtonDown(this InputAction action) => action.triggered && action.ReadValue<float>() > 0;
        public static bool GetKeyUp/GetButtonUp(this InputAction action) => action.triggered && action.ReadValue<float>() == 0;
    */
}