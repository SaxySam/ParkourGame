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
    public enum EOrientationMethod
    {
        TowardsCamera,
        TowardsMovement,
    }
    public enum EBonusOrientationMethod
    {
        None,
        TowardsGravity,
        TowardsGroundSlopeAndGravity,
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
        public KinematicCharacterMotor motor;

        [Header("Stable Movement")]
        public float maxStableMoveSpeed = 10f;
        public float stableMovementSharpness = 15;
        public float orientationSharpness = 10;
        public EOrientationMethod orientationMethod = EOrientationMethod.TowardsCamera;

        [Header("Air Movement")]
        public float maxAirMoveSpeed = 10f;
        public float airAccelerationSpeed = 5f;
        public float drag = 0.1f;

        [Header("Jumping")]
        public bool allowJumpingWhenSliding = false;
        public bool allowWallJump = false;
        public bool allowDoubleJump = false;
        public float jumpUpSpeed = 10f;
        public float jumpScalableForwardSpeed = 10f;
        public float jumpPreGroundingGraceTime = 0f;
        public float jumpPostGroundingGraceTime = 0f;

        [Header("Gravity")]
        
        public Vector3 gravity = new(0, -30f, 0);
        public EBonusOrientationMethod bonusOrientationMethod = EBonusOrientationMethod.TowardsGravity;
        public float bonusOrientationSharpness = 10;

        [Header("Misc")]
        public Transform meshRoot;
        public Transform cameraFollowPoint;
        public ECharacterState currentCharacterState { get; private set; }
        public float crouchedCapsuleHeight = 1f;
        public bool useFramePerfectRotation = false;
        public List<Collider> ignoredColliders = new();

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
            motor.CharacterController = this;
        }

        public void TransitionToState(ECharacterState newState)
        {
            ECharacterState tmpInitialState = currentCharacterState;
            OnStateExit(tmpInitialState, newState);
            currentCharacterState = newState;
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
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, motor.CharacterUp);

            switch (currentCharacterState)
            {
                case ECharacterState.Default:
                {
                    // Move and look inputs
                    _moveInputVector = cameraPlanarRotation * moveInputVector;

                    switch (orientationMethod)
                    {
                        case EOrientationMethod.TowardsCamera:
                            _lookInputVector = cameraPlanarDirection;
                            break;
                        case EOrientationMethod.TowardsMovement:
                            _lookInputVector = _moveInputVector.normalized;
                            break;
                    }

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
                            motor.SetCapsuleDimensions(0.5f, crouchedCapsuleHeight, crouchedCapsuleHeight * 0.5f);
                            meshRoot.localScale = new Vector3(1f, 0.5f, 1f);
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

        public void PostInputUpdate(float deltaTime, Vector3 cameraForward)
        {
            if (useFramePerfectRotation)
            {
                _lookInputVector = Vector3.ProjectOnPlane(cameraForward, motor.CharacterUp);

                Quaternion newRotation = default;
                HandleRotation(ref newRotation, deltaTime);
                meshRoot.rotation = newRotation;
            }
        }

        private void HandleRotation(ref Quaternion rot, float deltaTime)
        {
            if (_lookInputVector != Vector3.zero)
            {
                rot = Quaternion.LookRotation(_lookInputVector, motor.CharacterUp);
            }
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            // This is called before the motor does anything
        }


        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (currentCharacterState)
            {
                case ECharacterState.Default:
                    {
                        if (_lookInputVector != Vector3.zero && orientationSharpness > 0f)
                        {
                            // Smoothly interpolate from current to target look direction
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-orientationSharpness * deltaTime)).normalized;

                            // Set the current rotation (which will be used by the KinematicCharacterMotor)
                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, motor.CharacterUp);
                        }

                        Vector3 currentUp = (currentRotation * Vector3.up);
                        if (bonusOrientationMethod == EBonusOrientationMethod.TowardsGravity)
                        {
                            // Rotate from current up to invert gravity
                            Vector3 smoothedGravityDir = Vector3.Slerp(currentUp, -gravity.normalized, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                            currentRotation = Quaternion.FromToRotation(currentUp, smoothedGravityDir) * currentRotation;
                        }
                        else if (bonusOrientationMethod == EBonusOrientationMethod.TowardsGroundSlopeAndGravity)
                        {
                            if (motor.GroundingStatus.IsStableOnGround)
                            {
                                Vector3 initialCharacterBottomHemiCenter = motor.TransientPosition + (currentUp * motor.Capsule.radius);

                                Vector3 smoothedGroundNormal = Vector3.Slerp(motor.CharacterUp, motor.GroundingStatus.GroundNormal, 1 - Mathf.Exp(-bonusOrientationSharpness * deltaTime));
                                currentRotation = Quaternion.FromToRotation(currentUp, smoothedGroundNormal) * currentRotation;

                                // Move the position to create a rotation around the bottom hemi center instead of around the pivot
                                motor.SetTransientPosition(initialCharacterBottomHemiCenter + (currentRotation * Vector3.down * motor.Capsule.radius));
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

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (currentCharacterState)
            {
                case ECharacterState.Default:
                {

                    Vector3 targetMovementVelocity = Vector3.zero;
                    if (motor.GroundingStatus.IsStableOnGround)
                    {
                        // Reorient source velocity on current ground slope (this is because we don't want our smoothing to cause any velocity losses in slope changes)
                        currentVelocity = motor.GetDirectionTangentToSurface(currentVelocity, motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                        // Calculate target velocity
                        Vector3 inputRight = Vector3.Cross(_moveInputVector, motor.CharacterUp);
                        Vector3 reorientedInput = Vector3.Cross(motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                        targetMovementVelocity = reorientedInput * maxStableMoveSpeed;

                        // Smooth movement Velocity
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-stableMovementSharpness * deltaTime));
                    }
                    //Air Moveement
                    else
                    {
                        // Add move input
                        if (_moveInputVector.sqrMagnitude > 0f)
                        {
                            targetMovementVelocity = _moveInputVector * maxAirMoveSpeed;

                            // Prevent climbing on un-stable slopes with air movement
                            if (motor.GroundingStatus.FoundAnyGround)
                            {
                                Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(motor.CharacterUp, motor.GroundingStatus.GroundNormal), motor.CharacterUp).normalized;
                                targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                            }

                            Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, gravity);
                            currentVelocity += velocityDiff * airAccelerationSpeed * deltaTime;
                        }

                        // Gravity
                        currentVelocity += gravity * deltaTime;

                        // Drag
                        currentVelocity *= (1f / (1f + (drag * deltaTime)));
                        }

                    // Handle jumping
                    _jumpedThisFrame = false;
                    _timeSinceJumpRequested += deltaTime;
                    if (_jumpRequested)
                    {
                        // Handle double jump
                        if (allowDoubleJump)
                        {
                            if (_jumpConsumed && !_doubleJumpConsumed && (allowJumpingWhenSliding ? !motor.GroundingStatus.FoundAnyGround : !motor.GroundingStatus.IsStableOnGround))
                            {
                                motor.ForceUnground(0.1f);

                                // Add to the return velocity and reset jump state
                                currentVelocity += (motor.CharacterUp * jumpUpSpeed) - Vector3.Project(currentVelocity, motor.CharacterUp);
                                _jumpRequested = false;
                                _doubleJumpConsumed = true;
                                _jumpedThisFrame = true;
                            }
                        }

                        // See if we actually are allowed to jump
                        if (_canWallJump
                                || (!_jumpConsumed
                                && ((allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
                                || _timeSinceLastAbleToJump <= jumpPostGroundingGraceTime)))
                        {
                            // Calculate jump direction before ungrounding
                            Vector3 jumpDirection = motor.CharacterUp;
                            if (_canWallJump)
                            {
                                jumpDirection = _wallJumpNormal;
                            }
                            else if (motor.GroundingStatus.FoundAnyGround && !motor.GroundingStatus.IsStableOnGround)
                            {
                                jumpDirection = motor.GroundingStatus.GroundNormal;
                            }

                            // Makes the character skip ground probing/snapping on its next update. 
                            // If this line weren't here, the character would remain snapped to the ground when trying to jump. Try commenting this line out and see.
                            motor.ForceUnground(0.1f);

                            // Add to the return velocity and reset jump state
                            currentVelocity += (jumpDirection * jumpUpSpeed) - Vector3.Project(currentVelocity, motor.CharacterUp);
                            currentVelocity += (_moveInputVector * jumpScalableForwardSpeed);
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
            switch (currentCharacterState)
            {
                case ECharacterState.Default:
                {

                    // Handle jump-related values
                    {
                        // Handle jumping pre-ground grace period
                        if (_jumpRequested && _timeSinceJumpRequested > jumpPreGroundingGraceTime)
                        {
                            _jumpRequested = false;
                        }

                        // Handle jumping while sliding
                        if (allowJumpingWhenSliding ? motor.GroundingStatus.FoundAnyGround : motor.GroundingStatus.IsStableOnGround)
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
                        motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                        if (motor.CharacterCollisionsOverlap(
                                motor.TransientPosition,
                                motor.TransientRotation,
                                _probedColliders) > 0)
                        {
                            // If obstructions, just stick to crouching dimensions
                            motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
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

            switch (currentCharacterState)
            {
                case ECharacterState.Default:
                {
                    if (allowWallJump && !motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
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
            if (motor.GroundingStatus.IsStableOnGround && !motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!motor.GroundingStatus.IsStableOnGround && motor.LastGroundingStatus.IsStableOnGround)
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
            switch (currentCharacterState)
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
            switch (currentCharacterState)
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