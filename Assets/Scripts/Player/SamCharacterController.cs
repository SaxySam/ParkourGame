// https://docs.unity3d.com/ScriptReference/AddComponentMenu.html

using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using KinematicCharacterController;
using Unity.Cinemachine;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;
using Vector2 = UnityEngine.Vector2;


namespace SDK
{
    public enum ECharacterState
    {
        ParkourMode = 0,
        PhoneMode = 1
    }

    public enum EOrientationMethod
    {
        TowardsCamera = 0,
        TowardsMovement = 1,
    }
    
    public enum EJumpType
    {
        Impulse = 0,
        VariableHold = 1,
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
        public InputAction jumpAction;
        public InputAction crouchAction;
        public InputAction pauseAction;
        public InputAction lockMouseAction;
        public InputAction exitMouseAction;

        private FPlayerInputs(InputAction lookAction, InputAction moveAction, InputAction jumpAction, InputAction crouchAction, InputAction pauseAction, InputAction lockMouseAction, InputAction exitMouseAction)
        {
            this.lookAction = lookAction;
            this.moveAction = moveAction;
            this.jumpAction = jumpAction;
            this.crouchAction = crouchAction;
            this.pauseAction = pauseAction;
            this.lockMouseAction = lockMouseAction;
            this.exitMouseAction = exitMouseAction;
        }
    }

    [AddComponentMenu("Parkour/Parkour Character Controller")]
    public class SamCharacterController : MonoBehaviour, ICharacterController
    {
        [SerializeField] private KinematicCharacterMotor kinematicMotor;
        [field: SerializeField] public ECharacterState CurrentCharacterState { get; private set; } = ECharacterState.ParkourMode;

        private FPlayerInputs playerInputs;

        private PlayerInput playerInputComponent;


        [Header("Stable Movement")]
        public float maxStableMoveSpeed = 10f;
        public float readjustmentSpeed = 0.1f;
        public float accelerationRate = 5f;
        public float groundMovementFriction = 15;

        [EnumButtons]
        public EOrientationMethod orientationMethod = EOrientationMethod.TowardsMovement;
        public float TowardsCameraOrientationSharpness = 50;
        public float TowardsMovementOrientationSharpness = 10;
        [SerializeField] private CinemachineCamera playerThirdPersonCamera;

        private Vector3 moveInputVector;
        private Vector3 cameraPlanarDirection;
        private Quaternion cameraPlanarRotation;

        [Header("Jumping")]
        public bool allowJumpingWhenSliding = false;
        public bool allowWallJump = false;
        public bool allowDoubleJump = false;
        public float jumpScaleMultiplier = 1f;
        public float minJumpScalableUpSpeed = 2.5f;
        public float maxJumpScalableUpSpeed = 10f;
        public float minJumpScalableForwardSpeed = 2.5f;
        public float maxJumpScalableForwardSpeed = 10f;
        public float jumpPreGroundingGraceTime = 0f;
        public float jumpPostGroundingGraceTime = 0f;
        public EJumpType jumpType = EJumpType.Impulse;
        public float timeForMaxHeightJump = 0.5f;

        [Header("Launch")]

        //conditionals
        public bool enableLaunching = true;
        public float maxDistanceFromLedge = 5.0f;
        public float minPlayerVelocityToLaunch = 1.0f;
        public float minLedgeHeight = 5.0f;
        public float timeForMaxDistenceLaunch = 0.5f;
        public float launchScaleMultiplier = 1f;
        public float minLaunchScalableUpSpeed = 2.5f;
        public float maxLaunchScalableUpSpeed = 10f;
        public float minLaunchScalableForwardSpeed = 2.5f;
        public float maxLaunchScalableForwardSpeed = 10f;
        private bool _launchRequested = false;
        private bool _lauchActivated = false;
        private bool _launchButtonHeld = true;
        private bool _launchConsumed = false;
        private float _launchUpSpeed;
        private float _launchForwardSpeed;
        private float _holdDurationLaunch = 0;
        private Vector3 _LaunchDirection = Vector3.zero;



        [Header("Sliding")]
        [Tooltip("This is a slide")]
        public float slideSpeed;
        public bool _isSliding;


        [Header("Air Movement")]
        public float maxAirMoveSpeed = 10f;
        public float airAccelerationSpeed = 5f;
        public float drag = 0.1f;

        [Header("Gravity")]
        public Vector3 gravity = new(0, -30f, 0);
        public EBonusOrientationMethod bonusOrientationMethod = EBonusOrientationMethod.TowardsGravity;
        public float bonusOrientationSharpness = 10;

        [Header("Aniamtion")]
        public Animator playerAnimator;

        [Header("Misc")]
        public Transform meshRoot;
        public Transform cameraFollowPoint;
        public float crouchedCapsuleHeightMultiplier = 1f;
        public bool useFramePerfectRotation = false;
        public List<Collider> ignoredColliders = new();

        [Header("Privates")]
        private Collider[] _probedColliders = new Collider[8];
        private Vector3 _lookInputVector;
        private Vector3 _multipliedMoveInputVector;
        private float _linearSpeed;

        private float _holdDurationJump = 0f;
        private bool _jumpButtonHeld;
        private float _jumpUpSpeed;
        private float _jumpForwardSpeed;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private bool _doubleJumpConsumed = false;
        private bool _canWallJump = false;
        private Vector3 _wallJumpNormal;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;
        private Vector3 _jumpDirection = Vector3.zero;

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

            playerInputComponent.SwitchCurrentActionMap("ThirdPersonPlayer");

            playerInputs.lookAction = playerInputComponent.actions["Look"];
            playerInputs.lookAction.performed += Look;

            playerInputs.moveAction = playerInputComponent.actions["Move"];
            playerInputs.moveAction.performed += Move;
            playerInputs.moveAction.canceled += Move;

            playerInputs.jumpAction = playerInputComponent.actions["Jump"];
            playerInputs.jumpAction.performed += Jump;
            playerInputs.jumpAction.canceled += JumpCancelled;

            playerInputs.crouchAction = playerInputComponent.actions["Crouch"];
            playerInputs.crouchAction.performed += Crouch;
            playerInputs.crouchAction.canceled += Crouch;

            playerInputs.pauseAction = playerInputComponent.actions["Pause"];
            playerInputs.pauseAction.performed += Pause;

            playerInputs.lockMouseAction = playerInputComponent.actions["LeftClick"];
            playerInputs.lockMouseAction.performed += LockMouse;

            playerInputs.exitMouseAction = playerInputComponent.actions["Exit"];
            playerInputs.exitMouseAction.performed += Exit;
        }


        private void OnDisable()
        {


            playerInputs.lookAction.performed -= Look;

            playerInputs.moveAction.performed -= Move;
            playerInputs.moveAction.canceled -= Move;

            playerInputs.jumpAction.performed -= Jump;

            playerInputs.crouchAction.performed -= Crouch;
            playerInputs.crouchAction.canceled -= Crouch;

            playerInputs.lockMouseAction.performed -= LockMouse;

            playerInputs.exitMouseAction.performed -= Exit;
        }

        /// <summary>
        /// Start is called once before the first execution of Update after the MonoBehaviour is created
        /// </summary>
        private void Start()
        {
            // Assign to motor
            kinematicMotor.CharacterController = this;
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
        public void OnStateEnter(ECharacterState state, ECharacterState fromState)
        {
            switch (state)
            {
                case ECharacterState.ParkourMode:
                    {
                        break;
                    }
            }
        }

        /// <summary>
        /// Event when exiting a state
        /// </summary>
        public void OnStateExit(ECharacterState state, ECharacterState toState)
        {
            switch (state)
            {
                case ECharacterState.ParkourMode:
                    {
                        break;
                    }
            }
        }

        private void Look(InputAction.CallbackContext context)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        Quaternion cameraRotation = playerThirdPersonCamera.transform.rotation;

                        // Calculate camera direction and rotation on the character plane
                        cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.forward, kinematicMotor.CharacterUp).normalized;
                        if (cameraPlanarDirection.sqrMagnitude == 0f)
                        {
                            cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, kinematicMotor.CharacterUp).normalized;
                        }

                        cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, kinematicMotor.CharacterUp);
                        break;
                    }
            }
        }

        private void Move(InputAction.CallbackContext context)
        {
            // Clamp input
            moveInputVector = Vector3.ClampMagnitude(new Vector3(context.ReadValue<Vector2>().x, 0f, context.ReadValue<Vector2>().y), 1f);
            playerAnimator.SetFloat("PlayerSpeedX", moveInputVector.x);
            playerAnimator.SetFloat("PlayerSpeedZ", moveInputVector.z);
            playerAnimator.SetFloat("PlayerInputVelocity", moveInputVector.magnitude);
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
            }
        }

        private void JumpCancelled(InputAction.CallbackContext context)
        {
            _jumpButtonHeld = false;
            _holdDurationJump = 0;
            playerAnimator.SetTrigger("Fall");
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
                                _isCrouching = true;
                                kinematicMotor.SetCapsuleDimensions(kinematicMotor.CapsuleRadius,
                                kinematicMotor.CapsuleHeight / crouchedCapsuleHeightMultiplier,
                                kinematicMotor.CapsuleYOffset / crouchedCapsuleHeightMultiplier);
                                meshRoot.localScale = new Vector3(1f, 0.5f, 1f);

                                if (kinematicMotor.Velocity.magnitude >= slideSpeed)
                                {
                                    Debug.Log("<b><color=yellow>Sufficient Speed to Slide</b>");
                                    Slide();
                                }
                            }
                        }
                        else
                        {
                            _shouldBeCrouching = false;
                            _launchButtonHeld = false;
                            _holdDurationLaunch = 0;
                        }

                        break;
                    }
            }
        }

        private void CheckLaunchable()
        {
            if (!enableLaunching)
            {
                return;
            }

            if (!_isCrouching)
            {
                return;
            }

            if (!kinematicMotor.GroundingStatus.IsStableOnGround)
            {
                return;
            }

            if (kinematicMotor.BaseVelocity.magnitude <= minPlayerVelocityToLaunch)
            {
                return;
            }

            //send ray cast down
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, maxDistanceFromLedge))
            {
                Vector3 serfagePoint = hit.point + (Vector3.up * 0.01f);
                if (!Physics.Raycast(serfagePoint, kinematicMotor.CharacterForward, maxDistanceFromLedge))
                {
                    Vector3 ledgeCheckPoint = serfagePoint + (kinematicMotor.CharacterForward * maxDistanceFromLedge);
                    Debug.DrawLine(serfagePoint, ledgeCheckPoint, Color.red, 6f);
                    Debug.DrawLine(ledgeCheckPoint, ledgeCheckPoint + (Vector3.down * minLedgeHeight), Color.white, 6f);
                    if (!Physics.Raycast(ledgeCheckPoint, Vector3.down, minLedgeHeight))
                    {
                        //ledge is launchable 
                        Debug.Log($"launch ligit");
                        _launchRequested = true;
                        _launchConsumed = false;
                    }
                }
            }
        }

        private void Pause(InputAction.CallbackContext context)
        {
            GameManager.phoneOpenEvent?.Invoke();
        }

        private void LockMouse(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void Exit(InputAction.CallbackContext context)
        {
            Cursor.lockState = CursorLockMode.None;
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
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        _multipliedMoveInputVector = cameraPlanarRotation * moveInputVector;

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
                                    _lookInputVector = _multipliedMoveInputVector.normalized;

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

                        break;
                    }
            }
        }

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
                        Vector3 targetMovementVelocity = Vector3.zero;
                        if (kinematicMotor.GroundingStatus.IsStableOnGround)
                        {
                            // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                            currentVelocity = kinematicMotor.GetDirectionTangentToSurface(currentVelocity, kinematicMotor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                            // Calculate target velocity
                            Vector3 inputRight = Vector3.Cross(_multipliedMoveInputVector, kinematicMotor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(kinematicMotor.GroundingStatus.GroundNormal, inputRight).normalized * _multipliedMoveInputVector.magnitude;
                            // targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

                            float resultantVectorMagnitude = Mathf.Lerp(readjustmentSpeed, maxStableMoveSpeed, Mathf.InverseLerp(-1, 1, Vector3.Dot(currentVelocity, reorientedInput)));
                            targetMovementVelocity = reorientedInput * resultantVectorMagnitude;

                            if (currentVelocity != targetMovementVelocity)
                            {
                                targetMovementVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, Mathf.Pow(0.5f * accelerationRate, 2));
                            }

                            // Smooth movement Velocity
                            currentVelocity = Vector3.ClampMagnitude(Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-groundMovementFriction * deltaTime)), maxStableMoveSpeed);

                        }

                        //! Air Movement
                        else
                        {
                            // Add move input
                            if (_multipliedMoveInputVector.sqrMagnitude > 0f)
                            {
                                targetMovementVelocity = _multipliedMoveInputVector * maxAirMoveSpeed;

                                // Prevent climbing on un-stable slopes with air movement
                                if (kinematicMotor.GroundingStatus.FoundAnyGround)
                                {
                                    Vector3 perpendicularObstructionNormal = Vector3.Cross(Vector3.Cross(kinematicMotor.CharacterUp, kinematicMotor.GroundingStatus.GroundNormal), kinematicMotor.CharacterUp).normalized;
                                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpendicularObstructionNormal);
                                }

                                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, gravity);
                                currentVelocity += velocityDiff * airAccelerationSpeed * deltaTime;
                            }

                            // Gravity
                            currentVelocity += gravity * deltaTime;

                            // Drag
                            currentVelocity *= 1f / (1f + (drag * deltaTime));
                        }

                        //! Handle jumping
                        switch (jumpType)
                        {
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
                                        else if (kinematicMotor.GroundingStatus.FoundAnyGround && !kinematicMotor.GroundingStatus.IsStableOnGround)
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
                                        playerAnimator.SetTrigger("Jump");
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

                            case EJumpType.VariableHold:

                                _jumpedThisFrame = false;
                                _timeSinceJumpRequested += deltaTime;
                                if (_isCrouching)
                                {
                                    CheckLaunchable();
                                }

                                if (_jumpRequested)
                                {
                                    if (!_jumpConsumed)
                                    {
                                        if (kinematicMotor.GroundingStatus.IsStableOnGround || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)
                                        {
                                            //jump 
                                            _jumpDirection = Vector3.up;

                                            // Makes the character skip ground probing/snapping on its next update.
                                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                            kinematicMotor.ForceUnground(0.1f);

                                            _jumpRequested = false;
                                            _jumpConsumed = true;
                                            _jumpedThisFrame = true;
                                            playerAnimator.SetTrigger("Jump");
                                        }
                                    }
                                }

                                if (_jumpButtonHeld && _jumpConsumed)
                                {
                                    _holdDurationJump = Mathf.Clamp(_holdDurationJump + Time.fixedDeltaTime, 0, timeForMaxHeightJump);

                                    if (_holdDurationJump == timeForMaxHeightJump)
                                    {

                                        _jumpUpSpeed = 0;
                                        _jumpForwardSpeed = 0;
                                        _jumpButtonHeld = false;
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
                                            _LaunchDirection = Vector3.up;

                                            Debug.DrawRay(transform.position, _LaunchDirection, Color.red, 6f);

                                            // Makes the character skip ground probing/snapping on its next update.
                                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                            kinematicMotor.ForceUnground(0.1f);

                                            _launchRequested = false;
                                            _launchConsumed = true;
                                            _lauchActivated = true;
                                            playerAnimator.SetTrigger("Jump"); // to do change to valut trigger 
                                        }
                                    }
                                }

                                if (_launchConsumed && _lauchActivated)
                                {

                                    _holdDurationLaunch = Mathf.Clamp(_holdDurationLaunch + Time.fixedDeltaTime, 0,
                                        timeForMaxDistenceLaunch);

                                    if (_holdDurationLaunch >= timeForMaxDistenceLaunch)
                                    {

                                        _jumpUpSpeed = 0;
                                        _jumpForwardSpeed = 0;
                                        _jumpButtonHeld = false;
                                        _lauchActivated = false;
                                    }

                                    _launchUpSpeed = Mathf.Lerp(maxLaunchScalableUpSpeed * launchScaleMultiplier,
                                        minLaunchScalableUpSpeed * launchScaleMultiplier,
                                        1 - Mathf.InverseLerp(0, timeForMaxDistenceLaunch, _holdDurationLaunch));

                                    _launchForwardSpeed = Mathf.Lerp(
                                        maxLaunchScalableForwardSpeed * launchScaleMultiplier,
                                        minLaunchScalableForwardSpeed * launchScaleMultiplier,
                                        1 - Mathf.InverseLerp(0, timeForMaxDistenceLaunch, _holdDurationLaunch));

                                    currentVelocity += (_LaunchDirection * _launchUpSpeed) -
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
                                    _lauchActivated = false;
                                }

                                break;
                        }
                        break;
                    }
            }
            _linearSpeed = kinematicMotor.Velocity.magnitude;
            playerAnimator.SetFloat("MotorVelocity", kinematicMotor.Velocity.magnitude);
        }


        public void Slide()
        {

        }

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
                            kinematicMotor.SetCapsuleDimensions(kinematicMotor.CapsuleRadius, kinematicMotor.CapsuleHeight * crouchedCapsuleHeightMultiplier, kinematicMotor.CapsuleYOffset * crouchedCapsuleHeightMultiplier);
                            if (kinematicMotor.CharacterCollisionsOverlap(
                                    kinematicMotor.TransientPosition,
                                    kinematicMotor.TransientRotation,
                                    _probedColliders) > 0)
                            {
                                // If obstructions, just stick to crouching dimensions
                                kinematicMotor.SetCapsuleDimensions(kinematicMotor.CapsuleRadius, kinematicMotor.CapsuleHeight / crouchedCapsuleHeightMultiplier, kinematicMotor.CapsuleYOffset / crouchedCapsuleHeightMultiplier);
                            }
                            else
                            {
                                // If no obstructions, uncrouch
                                meshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }
                        break;
                    }
            }
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
            // We can wall jump only if we are not stable on ground and are moving against an obstruction

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
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // Handle landing and leaving ground
            if (kinematicMotor.GroundingStatus.IsStableOnGround && !kinematicMotor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!kinematicMotor.GroundingStatus.IsStableOnGround && kinematicMotor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        protected void OnLanded()
        {
            Debug.Log("<b><color=cyan>Landed</b>");
            playerAnimator.SetTrigger("Land");
        }

        protected void OnLeaveStableGround()
        {
            Debug.Log("<b><color=red>Left ground</b>");
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        break;
                    }
            }
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
        }
    }
}