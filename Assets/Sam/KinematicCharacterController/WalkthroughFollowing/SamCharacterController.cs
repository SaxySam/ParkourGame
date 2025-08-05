using KinematicCharacterController;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SDK
{

    [System.Serializable]
    public struct FPlayerInputs
    {
        public InputActionReference moveInputAction;
        public InputActionReference jumpInputAction;
        public InputActionReference sprintInputAction;
        public InputActionReference crouchInputAction;
        public Quaternion cameraRotation;

        // Constructor
        public FPlayerInputs(InputActionReference moveInputAction, InputActionReference jumpInputAction, InputActionReference sprintInputAction, InputActionReference crouchInputAction, Quaternion cameraRotation)
        {
            this.moveInputAction = moveInputAction;
            this.jumpInputAction = jumpInputAction;
            this.sprintInputAction = sprintInputAction;
            this.crouchInputAction = crouchInputAction;
            this.cameraRotation = cameraRotation;
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

        [Header("Misc")]
        public bool RotationObstruction;
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public Transform MeshRoot;

        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private void Start()
        {
            // Assign to motor
            Motor.CharacterController = this;
        }

        public void SetInputs(ref FPlayerInputs inputs)
        {
            // Clamp input

            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.moveInputAction.action.ReadValue<Vector2>().x, 0f, inputs.moveInputAction.action.ReadValue<Vector2>().y), 1f);

            // Calculate camera direction and rotation on the character plane
            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.cameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            // Move and look inputs
            _moveInputVector = cameraPlanarRotation * moveInputVector;
            _lookInputVector = cameraPlanarDirection;
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
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            // This is called after the motor has finished everything in its update
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
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            // This is called after every hit detected in the motor, to give you a chance to modify the HitStabilityReport any way you want
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            // This is called after the motor has finished its ground probing, but before PhysicsMover/Velocity/etc.... handling
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            // This is called by the motor when it is detecting a collision that did not result from a "movement hit".
        }
    }
}