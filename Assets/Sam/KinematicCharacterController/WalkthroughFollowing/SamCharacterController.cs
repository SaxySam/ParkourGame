using KinematicCharacterController;
using Unity.Cinemachine;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;


namespace SDK
{
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

        private PlayerInputActions _PlayerInput;

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
        public bool OrientTowardsGravity = true;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;

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

        [SerializeField] private CinemachineCamera _PlayerWalkCam;

        private InputAction _Move;
        private InputAction _Jump;
        private InputAction _Crouch;

        private void Awake()
        {
            _PlayerInput = new PlayerInputActions();
        }

        private void OnEnable()
        {
            _Move = _PlayerInput.Player.Move;
            _Move.Enable();
            _Move.performed += Move;
            _Move.canceled += Move;

            _Jump = _PlayerInput.Player.Jump;
            _Jump.Enable();
            _Jump.performed += Jump;

            _Crouch = _PlayerInput.Player.Crouch;
            _Crouch.Enable();
            _Crouch.performed += Crouch;






        }
        private void OnDisable()
        {
            _Move.Disable();
            _Move.performed -= Move;
            _Move.canceled -= Move;

            _Jump.Disable();
            _Jump.performed -= Jump;

            _Crouch.Disable();
            _Crouch.performed -= Crouch;

        }
        private void Start()
        {
            // Assign to motor
            Motor.CharacterController = this;
        }

        private void Crouch(InputAction.CallbackContext context)
        {
            // Crouching input
            if (!_shouldBeCrouching)
            {
                _shouldBeCrouching = true;

                if (!_isCrouching)
                {
                    _isCrouching = true;
                    Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                    MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                }
            }
            else 
            {
                _shouldBeCrouching = false;
            }
        }

        private void Jump(InputAction.CallbackContext context)
        {

            _timeSinceJumpRequested = 0f;
            _jumpRequested = true;
        }

        private void Move(InputAction.CallbackContext context)
        {
            // Clamp input
            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(_Move.ReadValue<Vector2>().x, 0f, _Move.ReadValue<Vector2>().y), 1f);

            Quaternion cameraRotation = _PlayerWalkCam.transform.rotation;

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(cameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            // Move and look inputs
            _moveInputVector = cameraPlanarRotation * moveInputVector;
            _lookInputVector = cameraPlanarDirection;
        }   


        public void SetInputs(ref FPlayerInputs inputs)
        {
           
        
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            // This is called before the motor does anything
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
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
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
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
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            // This is called after the motor has finished everything in its update

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
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            // This is called after when the motor wants to know if the collider can be collided with (or if we just go through it)
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
            if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
            {
                _canWallJump = true;
                _wallJumpNormal = hitNormal;
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
            _internalVelocityAdd += velocity;
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