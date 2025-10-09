// https://docs.unity3d.com/ScriptReference/AddComponentMenu.html


using System;
using System.Collections.Generic;
using KinematicCharacterController;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;


namespace SDK
{

    #region Enums and Structs
    public enum ECharacterState
    {
        ParkourMode = 0,
        PhoneMode = 1
    }


    public enum EOrientationMethod
    {
        TowardsCamera = 0,
        TowardsMovement = 1
    }


    public enum EJumpType
    {
        Impulse = 0,
        VariableHold = 1
    }


    public enum EBonusOrientationMethod
    {
        None = 0,
        TowardsGravity = 1,
        TowardsGroundSlopeAndGravity = 2
    }


    public struct FPlayerInputs
    {
        public InputAction LookAction;
        public InputAction MoveAction;
        public InputAction JumpAction;
        public InputAction CrouchAction;
        public InputAction PauseAction;
        public InputAction LockMouseAction;
        public InputAction ExitMouseAction;

        private FPlayerInputs(InputAction lookAction, InputAction moveAction, InputAction jumpAction, InputAction crouchAction, InputAction pauseAction, InputAction lockMouseAction, InputAction exitMouseAction)
        {
            this.LookAction = lookAction;
            this.MoveAction = moveAction;
            this.JumpAction = jumpAction;
            this.CrouchAction = crouchAction;
            this.PauseAction = pauseAction;
            this.LockMouseAction = lockMouseAction;
            this.ExitMouseAction = exitMouseAction;
        }
    }

    #endregion Enums and Structs

    #region Class Declarations

    [AddComponentMenu("Parkour Game/Parkour Character Controller")]
    [RequireComponent(typeof(KinematicCharacterMotor))]
    [HelpURL("https://assetstore.unity.com/packages/tools/physics/kinematic-character-controller-99131")]
    public class SamCharacterController : MonoBehaviour, ICharacterController
    {
        [SerializeField] private KinematicCharacterMotor kinematicMotor;
        [field: SerializeField] public ECharacterState CurrentCharacterState { get; private set; } = ECharacterState.ParkourMode;

        private FPlayerInputs _playerInputs;

        private PlayerInput _playerInputComponent;

        [Space(5)]
        [Header("Sensitivity")]
        //TODO - Look Sensitivity


        [Space(5)]
        [Header("Animation")]
        public Animator playerAnimator;
        private static readonly int PlayerSpeedX = Animator.StringToHash("PlayerSpeedX");
        private static readonly int PlayerSpeedZ = Animator.StringToHash("PlayerSpeedZ");
        private static readonly int PlayerInputVelocity = Animator.StringToHash("PlayerInputVelocity");
        private static readonly int Jumping = Animator.StringToHash("Jumping");
        private static readonly int Falling = Animator.StringToHash("Falling");
        private static readonly int Crouching = Animator.StringToHash("Crouching");
        private static readonly int Sliding = Animator.StringToHash("Sliding");
        private static readonly int MotorVelocity = Animator.StringToHash("MotorVelocity");
        private static readonly int Launch = Animator.StringToHash("Launch");
        private static readonly int Land = Animator.StringToHash("Land");


        [Space(5)]
        [Header("Stable Movement")]
        public float maxStableMoveSpeed = 10f;
        public float readjustmentSpeed = 0.1f;
        public float accelerationRate = 5f;
        public float groundMovementFriction = 15;

        [EnumButtons] public EOrientationMethod orientationMethod = EOrientationMethod.TowardsMovement;
        public float towardsCameraOrientationSharpness = 50;
        public float towardsMovementOrientationSharpness = 15;
        private float _internalOrientationSharpness;
        [SerializeField] private CinemachineCamera playerThirdPersonCamera;

        private float _internalMaxSpeed;
        private Vector3 _cameraPlanarDirection;
        private Quaternion _cameraPlanarRotation;
        private Vector3 _lookInputVector;
        private Vector3 _moveInputVector;
        private Vector3 _multipliedMoveInputVector;
        private float _linearSpeed;
        private Vector3 _vectorVelocity;
        private Vector3 _internalVelocityAdd = Vector3.zero;


        [Space(5)]
        [Header("Jumping")]
        public bool allowJumpingWhenSliding;
        public bool allowWallJump;
        public bool allowDoubleJump;
        public float jumpScaleMultiplier = 1f;
        public float minJumpScalableUpSpeed = 2.5f;
        public float maxJumpScalableUpSpeed = 10f;
        public float minJumpScalableForwardSpeed = 2.5f;
        public float maxJumpScalableForwardSpeed = 10f;
        public float jumpPreGroundingGraceTime;
        public float jumpPostGroundingGraceTime;
        [EnumButtons] public EJumpType jumpType = EJumpType.Impulse;
        public float timeForMaxHeightJump = 0.5f;

        private float _holdDurationJump;
        private bool _jumpButtonHeld;
        private float _jumpUpSpeed;
        private float _jumpForwardSpeed;
        private bool _jumpRequested;
        private bool _jumpConsumed;
        private bool _jumpedThisFrame;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump;
        private bool _doubleJumpConsumed;
        private bool _canWallJump;
        private Vector3 _wallJumpNormal;
        private Vector3 _jumpDirection = Vector3.zero;


        [Space(5)]
        [Header("Launch")]
        public bool enableLaunching = true;
        public float maxDistanceFromLedge = 5.0f;
        public float minPlayerVelocityToLaunch = 1.0f;
        public float minLedgeHeight = 5.0f;
        public float timeForMaxDistanceLaunch = 0.5f;
        public float launchScaleMultiplier = 1f;
        public float minLaunchScalableUpSpeed = 2.5f;
        public float maxLaunchScalableUpSpeed = 10f;
        public float minLaunchScalableForwardSpeed = 2.5f;
        public float maxLaunchScalableForwardSpeed = 10f;

        private bool _launchRequested;
        private bool _launchActivated;
        private bool _launchButtonHeld = true;
        private bool _launchConsumed;
        private float _launchUpSpeed;
        private float _launchForwardSpeed;
        private float _holdDurationLaunch;
        private Vector3 _launchDirection = Vector3.zero;


        [Space(5)]
        [Header("Crouch")]
        public float maxCrouchSpeed = 5f;
        public bool enableSquishyCrouch = true;
        public float crouchedCapsuleHeightDivisor = 2f;
        private bool _shouldBeCrouching;
        private bool _isCrouching;
        private Vector3 _normalCapsuleSize;
        private Vector3 _crouchedCapsuleSize;
        

        [Space(5)]
        [Header("Sliding")]
        public float startSlideSpeedThreshold;
        public float slideSpeedGain;
        public float minimumSlideSpeed;
        public float slideCooldownTime = 1f;
        public float decelerationRate;
        public float gravityMultiplier;
        public float movementRestrictionMultiplier;
        public float maxSlopeDetectionAngle = 45f;
        private RaycastHit _slopeOutHit;
        private bool _isSliding;
        private float _internalSlideSpeed;
        private float _lastSlideTime;


        [Header("Air Movement")]
        public float maxAirMoveSpeed = 10f;
        public float airAccelerationSpeed = 5f;
        public float drag = 0.1f;
        private float _internalMaxAirSpeed;


        [Space(5)]
        [Header("Gravity")]
        public Vector3 gravity = new(0, -30f, 0);
        public EBonusOrientationMethod bonusOrientationMethod = EBonusOrientationMethod.TowardsGravity;
        public float bonusOrientationSharpness = 10;


        [Space(5)]
        [Header("Misc")]
        public Transform meshRoot;
        public Transform cameraFollowPoint;
        public bool useFramePerfectRotation;
        public List<Collider> ignoredColliders = new();
        private readonly Collider[] _probedColliders = new Collider[8];

        #endregion Class Declarations

        #region Methods

        private void Awake()
        {
            _playerInputComponent = gameObject.GetComponent<PlayerInput>();
            if (_playerInputComponent == null)
            {
                Debug.Log("<b>Could not find PlayerInput Component!</b>");
            }
            Cursor.lockState = CursorLockMode.Locked;
        }

        #region OnEnable
        private void OnEnable()
        {
            kinematicMotor.CharacterController = this;

            _playerInputComponent.SwitchCurrentActionMap("ThirdPersonPlayer");

            _playerInputs.LookAction = _playerInputComponent.actions["Look"];
            _playerInputs.LookAction.performed += Look;

            _playerInputs.MoveAction = _playerInputComponent.actions["Move"];
            _playerInputs.MoveAction.performed += Move;
            _playerInputs.MoveAction.canceled += Move;

            _playerInputs.JumpAction = _playerInputComponent.actions["Jump"];
            _playerInputs.JumpAction.performed += Jump;
            _playerInputs.JumpAction.canceled += JumpCancelled;

            _playerInputs.CrouchAction = _playerInputComponent.actions["Crouch"];
            _playerInputs.CrouchAction.performed += Crouch;
            _playerInputs.CrouchAction.canceled += Crouch;

            _playerInputs.PauseAction = _playerInputComponent.actions["Pause"];
            _playerInputs.PauseAction.performed += Pause;

            _playerInputs.LockMouseAction = _playerInputComponent.actions["LeftClick"];
            _playerInputs.LockMouseAction.performed += LockMouse;

            _playerInputs.ExitMouseAction = _playerInputComponent.actions["Exit"];
            _playerInputs.ExitMouseAction.performed += Exit;
        }
        #endregion OnEnable

        #region OnDisable
        private void OnDisable()
        {
            _playerInputs.LookAction.performed -= Look;

            _playerInputs.MoveAction.performed -= Move;
            _playerInputs.MoveAction.canceled -= Move;

            _playerInputs.JumpAction.performed -= Jump;

            _playerInputs.CrouchAction.performed -= Crouch;
            _playerInputs.CrouchAction.canceled -= Crouch;

            _playerInputs.LockMouseAction.performed -= LockMouse;

            _playerInputs.ExitMouseAction.performed -= Exit;
        }
        #endregion OnDisable

        /// <summary>
        /// Start is called once before the first execution of Update after the MonoBehaviour is created
        /// </summary>
        private void Start()
        {
            // Assign to motor
            kinematicMotor.CharacterController = this;
            _internalSlideSpeed = slideSpeedGain;
            _internalOrientationSharpness = towardsMovementOrientationSharpness;
            _normalCapsuleSize = new Vector3(kinematicMotor.CapsuleRadius, kinematicMotor.CapsuleHeight, kinematicMotor.CapsuleYOffset);
            _crouchedCapsuleSize = new Vector3(kinematicMotor.CapsuleRadius,  kinematicMotor.CapsuleHeight / crouchedCapsuleHeightDivisor,  kinematicMotor.CapsuleYOffset / crouchedCapsuleHeightDivisor);
        }

        /// <summary>
        /// Handles movement state transitions and enter/exit callbacks
        /// </summary>
        public void TransitionToState(ECharacterState newState)
        {
            ECharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        /// <summary>
        /// Event when entering a state
        /// </summary>
        private static void OnStateEnter(ECharacterState state, ECharacterState fromState)
        {
            switch (state)
            {
                case ECharacterState.ParkourMode:
                case ECharacterState.PhoneMode:
                    {
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        private static void OnStateExit(ECharacterState state, ECharacterState toState)
        {
            switch (state)
            {
                case ECharacterState.ParkourMode:
                case ECharacterState.PhoneMode:
                    {
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(state), state, null);
            }
        }

        #region Input Action Methods
        private void Look(InputAction.CallbackContext context)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        Quaternion cameraRotation = playerThirdPersonCamera.transform.rotation;

                        // Calculate camera direction and rotation on the character plane
                        _cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.forward, kinematicMotor.CharacterUp).normalized;
                        if (_cameraPlanarDirection.sqrMagnitude == 0f)
                        {
                            _cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, kinematicMotor.CharacterUp).normalized;
                        }

                        _cameraPlanarRotation = Quaternion.LookRotation(_cameraPlanarDirection, kinematicMotor.CharacterUp);
                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Move(InputAction.CallbackContext context)
        {
            // Clamp input
            _moveInputVector = Vector3.ClampMagnitude(new Vector3(context.ReadValue<Vector2>().x, 0f, context.ReadValue<Vector2>().y), 1f);
            playerAnimator.SetFloat(PlayerSpeedX, _moveInputVector.x);
            playerAnimator.SetFloat(PlayerSpeedZ, _moveInputVector.z);
            playerAnimator.SetFloat(PlayerInputVelocity, _moveInputVector.magnitude);
        }

        private void Jump(InputAction.CallbackContext context)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested = true;
                        _jumpButtonHeld = true;
                        _jumpConsumed = false;
                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void JumpCancelled(InputAction.CallbackContext context)
        {
            _jumpButtonHeld = false;
            _holdDurationJump = 0;
            playerAnimator.SetBool(Jumping, false);
            playerAnimator.SetBool(Falling, true);
        }

        private void Crouch(InputAction.CallbackContext context)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        // Crouching input
                        if (!_shouldBeCrouching)
                        {
                            _shouldBeCrouching = true;
                            _launchButtonHeld = true;
                            _holdDurationLaunch = 0;

                            if (!_isCrouching)
                            {
                                if (kinematicMotor.Velocity.magnitude >= startSlideSpeedThreshold)
                                {
                                    if (Time.time - _lastSlideTime >= slideCooldownTime)
                                    {
                                        _isSliding = true;
                                        kinematicMotor.SetCapsuleDimensions(_crouchedCapsuleSize.x, _crouchedCapsuleSize.y, _crouchedCapsuleSize.z);
                                        _lastSlideTime = Time.time;
                                    }
                                }
                                else
                                {
                                    _isCrouching = true;
                                    kinematicMotor.SetCapsuleDimensions(_crouchedCapsuleSize.x, _crouchedCapsuleSize.y, _crouchedCapsuleSize.z);
                                    playerAnimator.SetBool(Crouching, true);

                                    if (enableSquishyCrouch)
                                    {
                                        meshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                                        playerAnimator.SetBool(Crouching, true);
                                    }
                                }

                            }
                        }
                        else
                        {
                            _shouldBeCrouching = false;
                            _launchButtonHeld = false;
                            _isSliding = false;
                            _internalSlideSpeed = slideSpeedGain;
                            _internalOrientationSharpness = towardsMovementOrientationSharpness;
                            playerAnimator.SetBool(Crouching, false);
                            playerAnimator.SetBool(Sliding, false);
                            _holdDurationLaunch = 0;
                        }

                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Pause(InputAction.CallbackContext context)
        {
            GameManager.PhoneOpenEvent?.Invoke();
        }

        private void LockMouse(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Exit(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.None;
        }

        #endregion Input Action Methods

        public void BeforeCharacterUpdate(float deltaTime)
        {
            // This is called before the motor does anything
        }

        public void PostInputUpdate(float deltaTime, Vector3 cameraForward)
        {
            if (!useFramePerfectRotation) return;
            
            _lookInputVector = Vector3.ProjectOnPlane(cameraForward, kinematicMotor.CharacterUp);

            Quaternion newRotation = default;
            HandleRotation(ref newRotation, deltaTime);
            meshRoot.rotation = newRotation;
        }

        #region Rotation

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
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        _multipliedMoveInputVector = _cameraPlanarRotation * _moveInputVector;

                        switch (orientationMethod)
                        {
                            case EOrientationMethod.TowardsCamera:
                                {
                                    _lookInputVector = _cameraPlanarDirection;

                                    if (_lookInputVector != Vector3.zero && towardsCameraOrientationSharpness > 0f)
                                    {
                                        // Smoothly interpolate from current to target look direction
                                        Vector3 smoothedLookInputDirection = Vector3.Slerp(kinematicMotor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-towardsCameraOrientationSharpness * deltaTime)).normalized;

                                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, kinematicMotor.CharacterUp);
                                    }
                                    break;
                                }
                            case EOrientationMethod.TowardsMovement:
                                {
                                    _lookInputVector = _multipliedMoveInputVector.normalized;

                                    if (_lookInputVector != Vector3.zero && _internalOrientationSharpness > 0f)
                                    {
                                        // Smoothly interpolate from current to target look direction
                                        Vector3 smoothedLookInputDirection = Vector3.Slerp(kinematicMotor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-_internalOrientationSharpness * deltaTime)).normalized;

                                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, kinematicMotor.CharacterUp);
                                    }
                                    break;
                                }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        Vector3 currentUp = currentRotation * Vector3.up;
                        switch (bonusOrientationMethod)
                        {
                            case EBonusOrientationMethod.TowardsGravity:
                            {
                                // Rotate from current up to invert gravity
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                                break;
                            }
                            case EBonusOrientationMethod.TowardsGroundSlopeAndGravity when kinematicMotor.GroundingStatus.IsStableOnGround:
                            {
                                Vector3 initialCharacterBottomHemiCenter = kinematicMotor.TransientPosition + (currentUp * kinematicMotor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(kinematicMotor.CharacterUp, kinematicMotor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                                kinematicMotor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * kinematicMotor.Capsule.radius));
                                break;
                            }
                            case EBonusOrientationMethod.TowardsGroundSlopeAndGravity:
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                                break;
                            }
                            case EBonusOrientationMethod.None:
                            default:
                            {
                                Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, Vector3.up, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                                break;
                            }
                        }

                        if (useFramePerfectRotation)
                        {
                            HandleRotation(ref currentRotation, deltaTime);
                        }

                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        #endregion Rotation

        #region Velocity
        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is where you tell your character what its velocity should be right now.
        /// This is the ONLY place where you can set the character's velocity
        /// </summary>
        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        //! Ground Movement
                        Vector3 targetMovementVelocity;
                        if (kinematicMotor.GroundingStatus.IsStableOnGround)
                        {
                            // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                            currentVelocity = kinematicMotor.GetDirectionTangentToSurface(currentVelocity, kinematicMotor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_multipliedMoveInputVector, kinematicMotor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(kinematicMotor.GroundingStatus.GroundNormal, inputRight).normalized * _multipliedMoveInputVector.magnitude;

                            //! Crouched Speeds
                            if (!_isSliding)
                            {
                                _internalMaxSpeed = _isCrouching ? maxCrouchSpeed : maxStableMoveSpeed;
                            }
                            else
                            {
                                _internalMaxSpeed = maxStableMoveSpeed + _internalSlideSpeed;
                            }
                            
                            float resultantVectorMagnitude = Mathf.Lerp(readjustmentSpeed, _internalMaxSpeed, Mathf.InverseLerp(-1, 1, Vector3.Dot(currentVelocity, reorientedInput)));
                            targetMovementVelocity = reorientedInput * resultantVectorMagnitude;

                            if (currentVelocity != targetMovementVelocity)
                            {
                                targetMovementVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, Mathf.Pow(0.5f * accelerationRate, 2));
                            }

                            // Smooth movement Velocity
                            currentVelocity = Vector3.ClampMagnitude(Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-groundMovementFriction * deltaTime)), _internalMaxSpeed);

                        }

                        //! Air Movement
                        else
                        {
                            // Add move input
                            if (_multipliedMoveInputVector.sqrMagnitude > 0f)
                            {
                                _internalMaxAirSpeed = !_isSliding ? maxAirMoveSpeed : maxAirMoveSpeed + _internalSlideSpeed;

                                targetMovementVelocity = _multipliedMoveInputVector * _internalMaxAirSpeed;

                                // Prevent climbing on unstable slopes with air movement
                                if (kinematicMotor.GroundingStatus.FoundAnyGround)
                                {
                                    Vector3 perpendicularObstructionNormal = Vector3.Cross(Vector3.Cross(kinematicMotor.CharacterUp, kinematicMotor.GroundingStatus.GroundNormal), kinematicMotor.CharacterUp).normalized;
                                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpendicularObstructionNormal);
                                }

                                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, gravity);
                                currentVelocity += velocityDiff * (airAccelerationSpeed * deltaTime);
                            }

                            //! Gravity
                            currentVelocity += !_isSliding ? gravity * deltaTime : deltaTime * gravityMultiplier * gravity;

                            //! Drag
                            currentVelocity *= 1f / (1f + (drag * deltaTime));
                        }


                        switch (_isSliding)
                        {
                            //! Sliding
                            case true:
                            {
                                Debug.Log("<b><color=yellow>Currently Sliding</b>");
                                playerAnimator.SetBool(Sliding, true);

                                _internalSlideSpeed = Mathf.Lerp(_internalSlideSpeed, 0, decelerationRate * Time.deltaTime);
                                
                                /*if (!OnSlope() || currentVelocity.y > -0.1f)
                                {
                                    
                                }
                                else
                                {
                                    _internalSlideSpeed = minimumSlideSpeed + (decelerationRate * Time.deltaTime);
                                }*/
                                
                                _internalOrientationSharpness = towardsMovementOrientationSharpness / movementRestrictionMultiplier;
                                if (_internalSlideSpeed <= minimumSlideSpeed)
                                {
                                    Debug.Log("<b><color=green>Finished Sliding</b>");
                                    _isSliding = false;
                                    playerAnimator.SetBool(Sliding, false);
                                    _internalSlideSpeed = slideSpeedGain;
                                    _internalOrientationSharpness = towardsMovementOrientationSharpness;
                                }

                                break;
                            }
                            case false when !_isCrouching:
                                kinematicMotor.SetCapsuleDimensions(_normalCapsuleSize.x, _normalCapsuleSize.y, _normalCapsuleSize.z);
                                break;
                        }


                        //! Handle jumping
                        currentVelocity = HandleJump(currentVelocity, deltaTime);
                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            _linearSpeed = kinematicMotor.Velocity.magnitude;
            _vectorVelocity = currentVelocity;
            playerAnimator.SetFloat(MotorVelocity, kinematicMotor.Velocity.magnitude);
        }

        #endregion Velocity

        private bool OnSlope()
        {
            if (!Physics.Raycast(transform.position,
                    Vector3.down,
                    out _slopeOutHit,
                    (kinematicMotor.CapsuleHeight * 0.5f) + 0.5f))
                return false;
            
            float angle = Vector3.Angle(Vector3.up, _slopeOutHit.normal);
            return angle < maxSlopeDetectionAngle && angle != 0;

        }

        private Vector3 GetSlopeMoveDirection(Vector3 moveDirection)
        {
            return Vector3.ProjectOnPlane(moveDirection, _slopeOutHit.normal).normalized;
        }

        #region Jumping

        private Vector3 HandleJump(Vector3 currentVelocity, float deltaTime)
        {
            switch (jumpType)
            {
                #region Impulse Jump
                case EJumpType.Impulse:

                    _jumpUpSpeed = maxJumpScalableUpSpeed;
                    _jumpedThisFrame = false;
                    _timeSinceJumpRequested += deltaTime;
                    if (_jumpRequested)
                    {
                        // Handle double jump
                        if (allowDoubleJump)
                        {
                            if (_jumpConsumed && !_doubleJumpConsumed && (allowJumpingWhenSliding ? !kinematicMotor.GroundingStatus.FoundAnyGround : !kinematicMotor.GroundingStatus.IsStableOnGround))
                            {
                                kinematicMotor.ForceUnground(0.1f);

                                // Add to the return velocity and reset jump state
                                currentVelocity += (kinematicMotor.CharacterUp * _jumpUpSpeed) - Vector3.Project(currentVelocity, kinematicMotor.CharacterUp);
                                _jumpRequested = false;
                                _doubleJumpConsumed = true;
                                _jumpedThisFrame = true;
                            }
                        }

                        // See if we actually are allowed to jump
                        if (_canWallJump
                            || (!_jumpConsumed && ((allowJumpingWhenSliding ? kinematicMotor.GroundingStatus.FoundAnyGround : kinematicMotor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)))
                        {
                            // Calculate jump direction before ungrounding
                            Vector3 jumpDirection = kinematicMotor.GroundingStatus.GroundNormal;
                            if (_canWallJump)
                            {
                                jumpDirection = _wallJumpNormal;
                                gameObject.transform.rotation = Quaternion.LookRotation(jumpDirection);
                            }
                            else if (kinematicMotor.GroundingStatus is { FoundAnyGround: true, IsStableOnGround: false })
                            {
                                jumpDirection = kinematicMotor.GroundingStatus.GroundNormal;
                            }

                            // Makes the character skip ground probing/snapping on its next update.
                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                            kinematicMotor.ForceUnground(0.1f);

                            // Add to the return velocity and reset jump state
                            currentVelocity += (jumpDirection * _jumpUpSpeed) - Vector3.Project(currentVelocity, kinematicMotor.CharacterUp);
                            currentVelocity += _multipliedMoveInputVector * maxJumpScalableForwardSpeed;
                            _jumpRequested = false;
                            _jumpConsumed = true;
                            _jumpedThisFrame = true;
                            playerAnimator.SetBool(Jumping, true);
                        }

                        // Reset wall jump
                        _canWallJump = false;
                    }

                    // Take into account additive velocity
                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                    {
                        currentVelocity += _internalVelocityAdd;
                        _internalVelocityAdd = Vector3.zero;
                    }
                    break;
                #endregion Impulse Jump

                #region Variable Jump
                case EJumpType.VariableHold:

                    _jumpedThisFrame = false;
                    _timeSinceJumpRequested += deltaTime;

                    if (_isSliding)
                    {
                        CheckLaunchable();
                    }

                    if (_jumpRequested)
                    {
                        if (!_jumpConsumed)
                        {
                            if (kinematicMotor.GroundingStatus.IsStableOnGround || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)
                            {
                                //Jump
                                _jumpDirection = Vector3.up;

                                // Makes the character skip ground probing/snapping on its next update.
                                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                kinematicMotor.ForceUnground(0.1f);

                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                                playerAnimator.SetBool(Jumping, true);
                            }
                        }
                    }

                    if (_jumpButtonHeld && _jumpConsumed)
                    {
                        _holdDurationJump = Mathf.Clamp(_holdDurationJump + Time.fixedDeltaTime, 0, timeForMaxHeightJump);

                        if (Mathf.Approximately(_holdDurationJump, timeForMaxHeightJump))
                        {
                            _jumpUpSpeed = 0;
                            _jumpForwardSpeed = 0;
                            _jumpButtonHeld = false;
                            playerAnimator.SetBool(Jumping, false);
                            playerAnimator.SetBool(Falling, true);
                        }

                        _jumpUpSpeed = Mathf.Lerp(maxJumpScalableUpSpeed * jumpScaleMultiplier, minJumpScalableUpSpeed * jumpScaleMultiplier, 1 - Mathf.InverseLerp(0, timeForMaxHeightJump, _holdDurationJump));

                        _jumpForwardSpeed = Mathf.Lerp(maxJumpScalableForwardSpeed * jumpScaleMultiplier, minJumpScalableForwardSpeed * jumpScaleMultiplier, 1 - Mathf.InverseLerp(0, timeForMaxHeightJump, _holdDurationJump));

                        currentVelocity += (_jumpDirection * _jumpUpSpeed) - Vector3.Project(currentVelocity, kinematicMotor.CharacterUp);
                        currentVelocity += _multipliedMoveInputVector * (maxJumpScalableForwardSpeed * _jumpForwardSpeed);

                        // Take into account additive velocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                    }

                    if (_launchRequested)
                    {
                        if (!_launchConsumed)
                        {
                            if (kinematicMotor.GroundingStatus.IsStableOnGround)
                            {
                                _launchDirection = Vector3.up;

                                Debug.DrawRay(transform.position, _launchDirection, Color.red, 6f);

                                // Makes the character skip ground probing/snapping on its next update.
                                // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                kinematicMotor.ForceUnground(0.1f);

                                _launchRequested = false;
                                _launchConsumed = true;
                                _launchActivated = true;
                                playerAnimator.SetTrigger(Launch);
                            }
                        }
                    }

                    if (_launchConsumed && _launchActivated)
                    {

                        _holdDurationLaunch = Mathf.Clamp(_holdDurationLaunch + Time.fixedDeltaTime, 0,
                            timeForMaxDistanceLaunch);

                        if (_holdDurationLaunch >= timeForMaxDistanceLaunch)
                        {
                            _jumpUpSpeed = 0;
                            _jumpForwardSpeed = 0;
                            _jumpButtonHeld = false;
                            _launchActivated = false;
                        }

                        _launchUpSpeed = Mathf.Lerp(maxLaunchScalableUpSpeed * launchScaleMultiplier,
                            minLaunchScalableUpSpeed * launchScaleMultiplier,
                            1 - Mathf.InverseLerp(0, timeForMaxDistanceLaunch, _holdDurationLaunch));

                        _launchForwardSpeed = Mathf.Lerp(
                            maxLaunchScalableForwardSpeed * launchScaleMultiplier,
                            minLaunchScalableForwardSpeed * launchScaleMultiplier,
                            1 - Mathf.InverseLerp(0, timeForMaxDistanceLaunch, _holdDurationLaunch));

                        currentVelocity += (_launchDirection * _launchUpSpeed) -
                                            Vector3.Project(currentVelocity, kinematicMotor.CharacterUp);
                        currentVelocity += _multipliedMoveInputVector *
                                           (maxLaunchScalableForwardSpeed * _launchForwardSpeed);

                        // Take into account additive velocity
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            currentVelocity += _internalVelocityAdd;
                            _internalVelocityAdd = Vector3.zero;
                        }
                    }
                    if (!_launchButtonHeld)
                    {
                        _launchActivated = false;
                    }

                    break;
                    #endregion Variable Jump

                default:
                    throw new ArgumentOutOfRangeException();
            }
            return currentVelocity;
        }

        #endregion Jumping

        #region Launching

        private void CheckLaunchable()
        {
            if (!enableLaunching) return;
            if (!_isSliding) return;
            if (!kinematicMotor.GroundingStatus.IsStableOnGround) return;
            if (kinematicMotor.BaseVelocity.magnitude <= minPlayerVelocityToLaunch) return;
            

            //send ray cast down
            if (!Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, maxDistanceFromLedge))
            {
                return;
            }

            Vector3 surfacePoint = hit.point + (Vector3.up * 0.01f);
            if (Physics.Raycast(surfacePoint, kinematicMotor.CharacterForward, maxDistanceFromLedge))
            {
                return;
            }

            Vector3 ledgeCheckPoint = surfacePoint + (kinematicMotor.CharacterForward * maxDistanceFromLedge);
            
            Debug.DrawLine(surfacePoint, ledgeCheckPoint, Color.red, 6f);
            Debug.DrawLine(ledgeCheckPoint, ledgeCheckPoint + (Vector3.down * minLedgeHeight), Color.white, 6f);
            
            if (Physics.Raycast(ledgeCheckPoint, Vector3.down, minLedgeHeight))
            {
                return;
            }

            //Ledge is Launchable
            Debug.Log("Launch Allowed");
            _launchRequested = true;
            _launchConsumed = false;
        }

        #endregion Launching

        #region Post-Update
        /// <summary>
        /// (Called by KinematicCharacterMotor during its update cycle)
        /// This is called after the character has finished its movement update
        /// </summary>
        public void AfterCharacterUpdate(float deltaTime)
        // This is called after the motor has finished everything in its update
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        // Handle jump-related values
                        {
                            // Handle jumping pre-ground grace period
                            if (_jumpRequested && _timeSinceJumpRequested > jumpPreGroundingGraceTime)
                            {
                                _jumpRequested = false;
                            }

                            // Handle jumping while sliding
                            if (allowJumpingWhenSliding ? kinematicMotor.GroundingStatus.FoundAnyGround : kinematicMotor.GroundingStatus.IsStableOnGround)
                            {
                                // If we're on a ground surface, reset jumping values
                                if (!_jumpedThisFrame)
                                {
                                    _doubleJumpConsumed = false;
                                    _jumpConsumed = false;
                                }
                                _timeSinceLastAbleToJump = 0f;
                            }
                            else
                            {
                                // Keep track of time since we were last able to jump (for grace period)
                                _timeSinceLastAbleToJump += deltaTime;
                            }
                        }

                        // Handle uncrouching
                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            // Do an overlap test with the character's standing height to see if there are any obstructions
                            kinematicMotor.SetCapsuleDimensions(_normalCapsuleSize.x, _normalCapsuleSize.y, _normalCapsuleSize.z);

                            if (kinematicMotor.CharacterCollisionsOverlap(kinematicMotor.TransientPosition, kinematicMotor.TransientRotation, _probedColliders) > 0)
                            {
                                // If obstructions, just stick to crouching dimensions
                                kinematicMotor.SetCapsuleDimensions(_crouchedCapsuleSize.x, _crouchedCapsuleSize.y, _crouchedCapsuleSize.z);
                            }
                            else
                            {
                                // If no obstructions, un-crouch
                                if (enableSquishyCrouch)
                                {
                                    meshRoot.localScale = new Vector3(1f, 1f, 1f);
                                }
                                _isCrouching = false;
                            }
                        }
                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #endregion Post-Update

        public bool IsColliderValidForCollisions(Collider coll)
        {
            // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)

            if (ignoredColliders.Count == 0)
            {
                return true;
            }

            return !ignoredColliders.Contains(coll);
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // This is called when the motor's ground probing detects a ground hit
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            // This is called when the motor's movement logic detects a hit
            // We can wall jump only if we are not stable on the ground and are moving against an obstruction

            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        if (allowWallJump && !kinematicMotor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                        {
                            _canWallJump = true;
                            _wallJumpNormal = hitNormal;
                        }
                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        #region Grounding

        public void PostGroundingUpdate(float deltaTime)
        {
            switch (kinematicMotor.GroundingStatus.IsStableOnGround)
            {
                // Handle landing and leaving ground
                case true when !kinematicMotor.LastGroundingStatus.IsStableOnGround:
                    OnLanded();
                    break;
                case false when kinematicMotor.LastGroundingStatus.IsStableOnGround:
                    OnLeaveStableGround();
                    break;
            }
        }

        private void OnLanded()
        {
            Debug.Log("<b><color=cyan>Landed</b>");
            playerAnimator.SetBool(Falling, false);
            playerAnimator.SetTrigger(Land);
        }

        private static void OnLeaveStableGround()
        {
            Debug.Log("<b><color=red>Left ground</b>");
        }

        #endregion Grounding

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
                case ECharacterState.PhoneMode:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                case ECharacterState.PhoneMode:
                    {
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
        }

        [ContextMenu("Do Something?")]
        private void DoSomething()
        {
            Debug.Log("<b><i><color=darkblue>You found me!</i></b>");
        }
    }
    
#endregion Methods
}