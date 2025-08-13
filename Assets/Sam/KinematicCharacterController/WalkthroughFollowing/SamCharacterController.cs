using System;
using System.Collections;
using System.Collections.Generic;
using KinematicCharacterController;
using KinematicCharacterController.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SDK
{
    public enum ECharacterState
    {
        Default,
    }

    public struct FPlayerInputs
    {
        public float moveAxisForward;
        public float moveAxisRight;
        public Quaternion cameraRotation;
        public bool jumpDown;
        public bool crouchDown;
        public bool crouchUp;

        public FPlayerInputs(float moveAxisForward, float moveAxisRight, Quaternion cameraRotation, bool jumpDown, bool crouchDown, bool crouchUp)
        {
            this.moveAxisForward = moveAxisForward;
            this.moveAxisRight = moveAxisRight;
            this.cameraRotation = cameraRotation;
            this.jumpDown = jumpDown;
            this.crouchDown = crouchDown;
            this.crouchUp = crouchUp;
        }
    }

    public class SamCharacterController : MonoBehaviour, ICharacterController
    {
        public FPlayerInputs playerInputs;
        public KinematicCharacterMotor Motor;

        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 10f;
        public float StableMovementSharpness = 15;
        public float OrientationSharpness = 10;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 10f;
        public float AirAccelerationSpeed = 5f;
        public float Drag = 0.1f;

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding = false;
        public bool AllowDoubleJump = false;
        public bool AllowWallJump = false;
        public float JumpSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;

        [Header("Misc")]
        public ECharacterState CurrentCharacterState { get; private set; }
        public Transform MeshRoot;
        public bool OrientTowardsGravity = true;
        public Vector3 Gravity = new(0, -30f, 0);
        public List<Collider> IgnoredColliders = new();

        private Collider[] _probedColliders = new Collider[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
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

        private void Start()
        {
            // Assign to motor
            Motor.CharacterController = this;
        }

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
                case ECharacterState.Default:
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
                case ECharacterState.Default:
                {
                    break;
                }
            }
        }

        public void SetInputs(ref FPlayerInputs inputs)
        {
            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.moveAxisRight, 0f, inputs.moveAxisForward), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            switch (CurrentCharacterState)
            {
                case ECharacterState.Default:
                {
                    // Move and look inputs
                    _moveInputVector = cameraPlanarRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;

                    // Jumping input
                    if (inputs.jumpDown)
                    {
                        _timeSinceJumpRequested = 0f;
                        _jumpRequested = true;
                    }

                    // Crouching input
                    if (inputs.crouchDown)
                    {
                        _shouldBeCrouching = true;

                        if (!_isCrouching)
                        {
                            _isCrouching = true;
                            Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                            MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                        }
                    }
                    else if (inputs.crouchUp)
                    {
                        _shouldBeCrouching = false;
                    }

                    break;
                }
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            // This is called before the motor does anything
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.Default:
                {
                    if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
                    {
                        // Smoothly interpolate from current to target look direction
                        Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                        // Set the current rotation (which will be used by the KinematicCharacterMotor)
                        currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                    }

                    if (OrientTowardsGravity)
                    {
                        // Rotate from current up to invert gravity
                        currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
                    }
                    break;
                }
            }
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.Default:
                {

                    Vector3 targetMovementVelocity = Vector3.zero;
                    if (Motor.GroundingStatus.IsStableOnGround)
                    {
                        // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                        currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                        // Smooth movement Velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                    }
                    else
                    {
                        // Add move input
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                            // Prevent climbing on un-stable slopes with air movement
                            if (Motor.GroundingStatus.FoundAnyGround)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                            }

                            Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                            currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                        }

                        // Gravity
                        currentVelocity += Gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                    }

                    // Handle jumping
                    _jumpedThisFrame = false;
                    _timeSinceJumpRequested += deltaTime;
                    if (_jumpRequested)
                    {
                        // Handle double jump
                        if (AllowDoubleJump)
                        {
                            if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                            {
                                Motor.ForceUnground(0.1f);

                                // Add to the return velocity and reset jump state
                                currentVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                _jumpRequested = false;
                                _doubleJumpConsumed = true;
                                _jumpedThisFrame = true;
                            }
                        }

                        // See if we actually are allowed to jump
                        if (_canWallJump
                                || (!_jumpConsumed
                                && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                                || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime)))
                        {
                            // Calculate jump direction before ungrounding
                            Vector3 jumpDirection = Motor.CharacterUp;
                            if (_canWallJump)
                            {
                                jumpDirection = _wallJumpNormal;
                            }
                            else if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                            {
                                jumpDirection = Motor.GroundingStatus.GroundNormal;
                            }

                            // Makes the character skip ground probing/snapping on its next update. 
                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                            Motor.ForceUnground(0.1f);

                            // Add to the return velocity and reset jump state
                            currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
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
                }
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        // This is called after the motor has finished everything in its update
        {
            switch (CurrentCharacterState)
            {
                case ECharacterState.Default:
                {

                    // Handle jump-related values
                    {
                        // Handle jumping pre-ground grace period
                        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                        {
                            _jumpRequested = false;
                        }

                        // Handle jumping while sliding
                        if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
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
                        Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                        if (Motor.CharacterCollisionsOverlap(
                                Motor.TransientPosition,
                                Motor.TransientRotation,
                                _probedColliders) > 0)
                        {
                            // If obstructions, just stick to crouching dimensions
                            Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                        }
                        else
                        {
                            // If no obstructions, uncrouch
                            MeshRoot.localScale = new Vector3(1f, 1f, 1f);
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
            if (IgnoredColliders.Contains(coll))
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
                case ECharacterState.Default:
                {
                    if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
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
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
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
                case ECharacterState.Default:
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
                case ECharacterState.Default:
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