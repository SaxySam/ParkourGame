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
        public InputAction lockMouseAction;
        public InputAction exitMouseAction;

        private FPlayerInputs(InputAction lookAction, InputAction moveAction, InputAction jumpAction, InputAction crouchAction, InputAction lockMouseAction, InputAction exitMouseAction)
        {
            this.lookAction = lookAction;
            this.moveAction = moveAction;
            this.jumpAction = jumpAction;
            this.crouchAction = crouchAction;
            this.lockMouseAction = lockMouseAction;
            this.exitMouseAction = exitMouseAction;
        }
    }

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

        [Header("Vaulting")]

        //conditionals
        public bool enableVaulting = true;
        public float maxHightFromLedge = 20.0f;
        public float maxDistanceFromLedge = 5.0f;
        public float minPlayerVelocityToVault = 1.0f;
        public float minLedgeHight = 5.0f;

        private bool _vaultRequested = false;

        //physics 
        public float vaultUpAngle = 15f;

        [Header("Air Movement")]
        public float maxAirMoveSpeed = 10f;
        public float airAccelerationSpeed = 5f;
        public float drag = 0.1f;

        [Header("Gravity")]
        public Vector3 gravity = new(0, -30f, 0);
        public EBonusOrientationMethod bonusOrientationMethod = EBonusOrientationMethod.TowardsGravity;
        public float bonusOrientationSharpness = 10;

        [Header("Misc")]
        public Transform meshRoot;
        public Transform cameraFollowPoint;
        public float crouchedCapsuleHeight = 1f;
        public bool useFramePerfectRotation = false;
        public List<Collider> ignoredColliders = new();

        [Header("Privates")]
        private Collider[] _probedColliders = new Collider[8];
        private Vector3 _lookInputVector;
        private Vector3 _moveInputVector;
        private float _linearSpeed;
        private bool _jumpButtonHeld;
        private float _holdDuration = 0f;
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
            playerInputComponent.SwitchCurrentActionMap("Player");

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
            moveInputVector = Vector3.ClampMagnitude(new Vector3(playerInputs.moveAction.ReadValue<Vector2>().x, 0f, playerInputs.moveAction.ReadValue<Vector2>().y), 1f);
        }

        private void Jump(InputAction.CallbackContext context)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.ParkourMode:
                    {
                        CheckValuable();
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested = true;
                        _jumpButtonHeld = true;
                        _jumpConsumed = false;
                        break;
                    }
            }
        }

        private void CheckValuable()
        {
            if (!enableVaulting)
            {
                return;
            }

            if (!kinematicMotor.GroundingStatus.IsStableOnGround)
            {
                Debug.Log($"not on stable ground");
                //send ray cast down
                RaycastHit hit;
                Debug.DrawLine(transform.position, transform.position + (Vector3.down * maxHightFromLedge), Color.blue, 6f);
                if (Physics.Raycast(transform.position, Vector3.down, out hit, maxDistanceFromLedge))
                {
                    Debug.Log($"right dist from ground");
                    Vector3 serfagePoint = hit.point + (Vector3.up * 0.01f);
                    Vector3 ledgeCheckPoint = serfagePoint + (kinematicMotor.CharacterForward * maxDistanceFromLedge);
                    Debug.DrawLine(serfagePoint, ledgeCheckPoint, Color.red, 6f);
                    Debug.DrawLine(ledgeCheckPoint, ledgeCheckPoint + (Vector3.down * minLedgeHight), Color.white, 6f);
                    if (!Physics.Raycast(ledgeCheckPoint, Vector3.down, minLedgeHight))
                    {
                        //ledge is valuable do vault
                        Debug.Log($"leage ligit");
                        _vaultRequested = true;

                    }
                }
            }
        }

        private void JumpCancelled(InputAction.CallbackContext context)
        {
            _jumpButtonHeld = false;
            _holdDuration = 0;
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

                            if (!_isCrouching)
                            {
                                _isCrouching = true;
                                kinematicMotor.SetCapsuleDimensions(0.5f, crouchedCapsuleHeight, crouchedCapsuleHeight * 0.5f);
                                meshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                            }
                        }
                        else
                        {
                            _shouldBeCrouching = false;
                        }

                        break;
                    }
            }
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

                        }

                        //! Air Movement
                        else
                        {
                            // Add move input
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                targetMovementVelocity = _moveInputVector * maxAirMoveSpeed;

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
                                        currentVelocity += _moveInputVector * maxJumpScalableForwardSpeed;
                                        _jumpRequested = false;
                                        _jumpConsumed = true;
                                        _jumpedThisFrame = true;
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
                                if (_jumpRequested)
                                {
                                    if (!_jumpConsumed)
                                    {
                                        if (_vaultRequested)
                                        {
                                            
                                            _jumpDirection = Vector3.Slerp(kinematicMotor.CharacterUp, kinematicMotor.CharacterForward, Mathf.InverseLerp(0, 90, vaultUpAngle));
                                            Debug.DrawRay(transform.position, _jumpDirection, Color.red, 60f);

                                            // Makes the character skip ground probing/snapping on its next update.
                                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                            kinematicMotor.ForceUnground(0.1f);

                                            _jumpRequested = false;
                                            _vaultRequested = false;
                                            _jumpConsumed = true;
                                            _jumpedThisFrame = true;
                                        }
                                        else if (kinematicMotor.GroundingStatus.IsStableOnGround || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)
                                        {
                                            //jump 
                                            _jumpDirection = Vector3.up;

                                            // Makes the character skip ground probing/snapping on its next update.
                                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                                            kinematicMotor.ForceUnground(0.1f);

                                            _jumpRequested = false;
                                            _jumpConsumed = true;
                                            _jumpedThisFrame = true;
                                        }
                                    }
                                }
                                
                                if (_jumpButtonHeld && _jumpConsumed)
                                {
                                    _holdDuration = Mathf.Clamp(_holdDuration + Time.fixedDeltaTime, 0, timeForMaxHeightJump);

                                    if (_holdDuration == timeForMaxHeightJump)
                                    {
                                        _jumpUpSpeed = 0;
                                        _jumpForwardSpeed = 0;
                                        _jumpButtonHeld = false;
                                    }

                                    _jumpUpSpeed = Mathf.Lerp(maxJumpScalableUpSpeed * jumpScaleMultiplier, minJumpScalableUpSpeed * jumpScaleMultiplier, 1 - Mathf.InverseLerp(0, timeForMaxHeightJump, _holdDuration));

                                    _jumpForwardSpeed = Mathf.Lerp(maxJumpScalableForwardSpeed * jumpScaleMultiplier, minJumpScalableForwardSpeed * jumpScaleMultiplier, 1 - Mathf.InverseLerp(0, timeForMaxHeightJump, _holdDuration));

                                    currentVelocity += (_jumpDirection * _jumpUpSpeed) - Vector3.Project(currentVelocity, kinematicMotor.CharacterUp);
                                    currentVelocity += _moveInputVector * (maxJumpScalableForwardSpeed * _jumpForwardSpeed);

                                    // Take into account additive velocity
                                    if (_internalVelocityAdd.sqrMagnitude > 0f)
                                    {
                                        currentVelocity += _internalVelocityAdd;
                                        _internalVelocityAdd = Vector3.zero;
                                    }

                                }
                            break;
                            
                        }
                        break;
                    }
            }
            _linearSpeed = currentVelocity.magnitude;
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
                            kinematicMotor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (kinematicMotor.CharacterCollisionsOverlap(
                                    kinematicMotor.TransientPosition,
                                    kinematicMotor.TransientRotation,
                                    _probedColliders) > 0)
                            {
                                // If obstructions, just stick to crouching dimensions
                                kinematicMotor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
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