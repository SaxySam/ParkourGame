using System.Collections.Generic;
using KinematicCharacterController;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Phone;

namespace PhotoCamera
{
    public enum EOrientationMethod
    {
        TowardsCamera = 0,
        TowardsMovement = 1,
    }

    public enum EBonusOrientationMethod
    {
        None = 0,
        TowardsGravity = 1,
        TowardsGroundSlopeAndGravity = 2,
    }

    public struct FPlayerInputs
    {
        public InputAction lookAction;
        public InputAction moveAction;
        public InputAction TakePhotoAction;
        public InputAction PauseAction;

        private FPlayerInputs(InputAction lookAction, InputAction moveAction, InputAction TakePhotoAction, InputAction PauseAction)
        {
            this.lookAction = lookAction;
            this.moveAction = moveAction;
            this.TakePhotoAction = TakePhotoAction;
            this.PauseAction = PauseAction;
        }
    }

    [AddComponentMenu("Parkour Game/FirstPersonMovement")]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    [HelpURL("https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131")]
    public class FirstPersonMovement : MonoBehaviour, ICharacterController
    {
        [SerializeField] private KinematicCharacterMotor kinematicMotor;
        private FPlayerInputs playerInputs;

        private PlayerInput playerInputComponent;

        [Header("Stable Movement")]
        public float maxStableMoveSpeed = 10f;
        public float readjustmentSpeed = 0.1f;
        public float accelerationRate = 5f;
        public float groundMovementFriction = 15;
        public EOrientationMethod orientationMethod = EOrientationMethod.TowardsMovement;
        public float TowardsCameraOrientationSharpness = 50;
        public float TowardsMovementOrientationSharpness = 10;
        [SerializeField] private CinemachineCamera playerThirdPersonCamera;

        private Vector3 moveInputVector;
        private Vector3 cameraPlanarDirection;
        private Quaternion cameraPlanarRotation;

        [Header("Gravity")]
        public Vector3 gravity = new(0, -30f, 0);
        public EBonusOrientationMethod bonusOrientationMethod = EBonusOrientationMethod.TowardsGravity;
        public float bonusOrientationSharpness = 10;

        [Header("Misc")]
        public Transform meshRoot;
        public Transform cameraFollowPoint;
        public bool useFramePerfectRotation = false;
        public List<Collider> ignoredColliders = new();

        [Header("Privates")]
        private Collider[] _probedColliders = new Collider[8];
        private Vector3 _lookInputVector;
        private Vector3 _moveInputVector;
        private float _linearSpeed;
        private PhotoCamera photoCamera;

        private void Awake()
        {
            playerInputComponent = gameObject.GetComponent<PlayerInput>();
            if (playerInputComponent == null)
            {
                Debug.Log("did not get Player input componit ");
            }
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            kinematicMotor.CharacterController = this;

            playerInputComponent.SwitchCurrentActionMap("FirstPersonCamera");

            // playerInputs.lookAction = playerInputComponent.actions["Look"];
            playerInputs.lookAction = playerInputComponent.actions["Look"];
            playerInputs.lookAction.performed += Look;

            playerInputs.moveAction = playerInputComponent.actions["Move"];
            playerInputs.moveAction.performed += Move;
            playerInputs.moveAction.canceled += Move;

            playerInputs.TakePhotoAction = playerInputComponent.actions["TakePhoto"];
            playerInputs.TakePhotoAction.performed += TakePhoto;

            playerInputs.PauseAction = playerInputComponent.actions["Pause"];
            playerInputs.PauseAction.performed += Pause;
        }

        private void OnDisable()
        {

            playerInputs.lookAction.performed -= Look;

            playerInputs.moveAction.performed -= Move;
            playerInputs.moveAction.canceled -= Move;

            playerInputs.TakePhotoAction.performed -= TakePhoto;

            playerInputs.PauseAction.performed -= Pause;
        }

        /// <summary>
        /// Start is called once before the first execution of Update after the MonoBehaviour is created
        /// </summary>
        private void Start()
        {
            // Assign to motor
            photoCamera = FindFirstObjectByType<PhotoCamera>();
        }

        private void Look(InputAction.CallbackContext context)
        {
            Quaternion cameraRotation = playerThirdPersonCamera.transform.rotation;

            // Calculate camera direction and rotation on the character plane
            cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.forward, kinematicMotor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, kinematicMotor.CharacterUp).normalized;
            }

            cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, kinematicMotor.CharacterUp);

        }

        private void Move(InputAction.CallbackContext context)
        {
            // Clamp input
            moveInputVector = Vector3.ClampMagnitude(new Vector3(playerInputs.moveAction.ReadValue<Vector2>().x, 0f, playerInputs.moveAction.ReadValue<Vector2>().y), 1f);
        }

        private void TakePhoto(InputAction.CallbackContext context)
        {
            Debug.Log("Take Photo");
            if (photoCamera != null)
            {
                photoCamera.TakePhoto();
            }
        }

        private void Pause(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.None;
            // _isPaused = !_isPaused;

            GameManager.phoneOpenEvent?.Invoke();
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            // This is called before the motor does anything
        }

        public void PostInputUpdate(float deltaTime, Vector3 cameraForward)
        {
            if (useFramePerfectRotation)
            {
                _lookInputVector = Vector3.ProjectOnPlane(cameraForward, kinematicMotor.CharacterUp);

                Quaternion newRotation = default;
                HandleRotation(ref newRotation, deltaTime);
                meshRoot.rotation = newRotation;
            }
        }

        private void HandleRotation(ref Quaternion rot, float deltaTime)
        {
            if (_lookInputVector != Vector3.zero)
            {
                rot = Quaternion.LookRotation(_lookInputVector, kinematicMotor.CharacterUp);
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its rotation should be right now.
        /// This is the ONLY place where you should set the character's rotation
        /// </summary>
        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            _moveInputVector = cameraPlanarRotation * moveInputVector;

            switch (orientationMethod)
            {
                case EOrientationMethod.TowardsCamera:
                {
                    _lookInputVector = cameraPlanarDirection;

                    if (_lookInputVector != Vector3.zero && TowardsCameraOrientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(kinematicMotor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-TowardsCameraOrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, kinematicMotor.CharacterUp);
                    }
                    break;
                }
                case EOrientationMethod.TowardsMovement:
                {
                    _lookInputVector = _moveInputVector.normalized;

                    if (_lookInputVector != Vector3.zero && TowardsMovementOrientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(kinematicMotor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-TowardsMovementOrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, kinematicMotor.CharacterUp);
                    }
                    break;
                }
            }

            Vector3 currentUp = currentRotation * Vector3.up;
            if (bonusOrientationMethod == EBonusOrientationMethod.TowardsGravity)
            {
                // Rotate from current up to invert gravity
                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
            }
            else if (bonusOrientationMethod == EBonusOrientationMethod.TowardsGroundSlopeAndGravity)
            {
                if (kinematicMotor.GroundingStatus.IsStableOnGround)
                {
                    Vector3 initialCharacterBottomHemiCenter = kinematicMotor.TransientPosition + (currentUp * kinematicMotor.Capsule.radius);

                    Vector3 smoothedGroundNormal = Vector3.Slerp(kinematicMotor.CharacterUp, kinematicMotor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                    // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                    kinematicMotor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * kinematicMotor.Capsule.radius));
                }
                else
                {
                    Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                    currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                }
            }
            else
            {
                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
            }

            if (useFramePerfectRotation)
            {
                HandleRotation(ref currentRotation, deltaTime);
            }
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now.
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            //! Ground Movement
            Vector3 targetMovementVelocity = Vector3.zero;

            // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
            currentVelocity = kinematicMotor.GetDirectionTangentToSurface(currentVelocity, kinematicMotor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

            // Calculate target velocity
            Vector3 inputRight = Vector3.Cross(_moveInputVector, kinematicMotor.CharacterUp);
            Vector3 reorientedInput = Vector3.Cross(kinematicMotor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
            // targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

            float resultantVectorMagnitude = Mathf.Lerp(readjustmentSpeed, maxStableMoveSpeed, Mathf.InverseLerp(-1, 1, Vector3.Dot(currentVelocity, reorientedInput)));
            targetMovementVelocity = reorientedInput * resultantVectorMagnitude;

            if (currentVelocity != targetMovementVelocity)
            {
                targetMovementVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, Mathf.Pow(0.5f * accelerationRate, 2));
            }

            // Smooth movement Velocity
            currentVelocity = Vector3.ClampMagnitude(Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-groundMovementFriction * deltaTime)), maxStableMoveSpeed);
            _linearSpeed = currentVelocity.magnitude;
        }

        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        {
            // This is called after the motor has finished everything in its update
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)

            if (ignoredColliders.Count == 0)
            {
                return true;
            }

            if (ignoredColliders.Contains(coll))
            {
                return false;
            }
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // This is called when the motor's ground probing detects a ground hit
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // This is called when the motor's movement logic detects a hit
        }

        public void PostGroundingUpdate(float deltaTime)
        {

        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
        }
    }
}