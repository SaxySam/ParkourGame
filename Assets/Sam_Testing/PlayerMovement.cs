using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Object References")]
    [Space(10)]
    [SerializeField] private CharacterController _characterController;
    [SerializeField] private Rigidbody _rigidBody;

    [Header("Input Actions")]
    [Space(10)]

    [SerializeField] private InputActionReference _moveInputActionRef;
    [SerializeField] private InputActionReference _jumpInputActionRef;
    [SerializeField] private InputActionReference _sprintInputActionRef;
    [SerializeField] private InputActionReference _crouchInputActionRef;

    [Header("Player Movement")]
    [Space(10)]

    [SerializeField] private float _crouchSpeed;
    [SerializeField] private float _walkSpeed;
    [SerializeField] private float _sprintSpeed;
    private float _playerSpeed;
    [SerializeField] private Vector3 _inputVector = Vector3.zero;
    [SerializeField] private Vector3 _movementVector = Vector3.zero;
    [SerializeField] private bool _isCrouching;
    [SerializeField] private bool _isSprinting;
    [SerializeField] private bool _normalizeMovement = false;

    [Header("Jumping & Ground Checking")]
    [Space(10)]
    [SerializeField] private float _jumpHeight;
    [SerializeField] private Vector3 _verticalVelocity;
    [SerializeField] private LayerMask _groundMask;
    [SerializeField] private Transform _groundCheck;
    [Range(0.1f, 1)] public float _groundDistance = 0.2f;
    [SerializeField] private bool _isGrounded;
    private float _gravity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _characterController = gameObject.GetComponent<CharacterController>();
        _rigidBody = gameObject.GetComponent<Rigidbody>();
        _playerSpeed = _walkSpeed;
        _gravity = Physics.gravity.y;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        _inputVector = context.ReadValue<Vector2>();
    }
    public void OnJump(InputAction.CallbackContext context)
    {
        Jump();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        _isSprinting = context.ReadValueAsButton();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        _isCrouching = context.ReadValueAsButton();
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);

        if (_isGrounded && _verticalVelocity.y < 0)
        {
            _verticalVelocity.y = -2f;
        }

        _inputVector.x = _moveInputActionRef.action.ReadValue<Vector2>().x;
        _inputVector.z = _moveInputActionRef.action.ReadValue<Vector2>().y;

        MovePlayer();
    }

    private void MovePlayer()
    {
        if (_isSprinting)
        {
            _playerSpeed = Mathf.Lerp(_playerSpeed, _sprintSpeed, Time.deltaTime * 2);
            if (Mathf.Approximately(_playerSpeed, _sprintSpeed))
            {
                _playerSpeed = _sprintSpeed;
            }
        }

        else if (_isCrouching)
        {
            _playerSpeed = Mathf.Lerp(_playerSpeed, _crouchSpeed, Time.deltaTime * 5);
            if (Mathf.Approximately(_playerSpeed, _crouchSpeed))
            {
                _playerSpeed = _crouchSpeed;
            }
        }
        else
        {
            _playerSpeed = Mathf.Lerp(_playerSpeed, _walkSpeed, Time.deltaTime * 3);
            if (Mathf.Approximately(_playerSpeed, _walkSpeed))
            {
                _playerSpeed = _walkSpeed;
            }
        }

        _movementVector = _normalizeMovement ? (transform.right * _inputVector.x + transform.forward * _inputVector.z).normalized : (transform.right * _inputVector.x + transform.forward * _inputVector.z);
        _characterController.Move(_movementVector * _playerSpeed * Time.deltaTime);
        _verticalVelocity.y += (_gravity * Time.deltaTime) * 2;
        _characterController.Move(_verticalVelocity * Time.deltaTime);
    }

    private void Jump()
    {
        if (_isGrounded)
        {
            _verticalVelocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
            // _rigidBody.AddForce(Vector2.up * _jumpHeight, ForceMode.Impulse);
            // _isGrounded = false;

            // _rigidBody.linearVelocity = new Vector2(_rigidBody.linearVelocity.x, _jumpHeight);
            // _rigidBody. = _rigidBody.linearVelocity.y > 0 ? 1f : 2.5f;
        }
    }

}
