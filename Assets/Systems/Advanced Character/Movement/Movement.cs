using System;
using Snowy.Input;
using Systems;
using Systems.Advanced_Character.Movement;
using UnityEngine;

public enum Controller { CharacterController, Rigidbody }

public class BasicMovement : MovementBrain
{
    [SerializeField] private Controller controllerType;

    [Space(10)] [Header("Settings")] [SerializeField]
    private float height = 2f;

    [SerializeField] private float width = 0.5f;
    [SerializeField] private Vector3 center = Vector3.up * .5f;
    [SerializeField] private float groundDistance = 2f;
    [SerializeField] private float groundCheckerRadius = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private bool canJump = true;
    [SerializeField] private bool canAirControl = true;
    [SerializeField] private bool canSprint = true;

    [Header("Movement")] [SerializeField] private float speed = 5f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float airControl = 0.5f;

    private float _currentSpeed = 5f;
    private bool _isGrounded;
    private readonly float _groundDrag = 6f;
    private Vector3 _velocity;
    private Rigidbody _rb;
    private CharacterController _cc;
    private InputManager _input;

    private void Start()
    {
        switch (controllerType)
        {
            case Controller.CharacterController:
                SetUpCharacterController();
                break;
            case Controller.Rigidbody:
                SetUpRigidbody();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _input = GetComponentInChildren<InputManager>();
        _input.OnSprint += (state) =>
        {
            if (!canSprint || state == ButtonState.Released || state == ButtonState.None || !_isGrounded)
            {
                _currentSpeed = speed;
                return;
            }

            _currentSpeed = sprintSpeed;
        };
        _currentSpeed = speed;
    }

    void SetUpCharacterController()
    {
        if (!TryGetComponent(out _cc))
            _cc = gameObject.AddComponent<CharacterController>();

        _cc.skinWidth = 0.0001f;
        _cc.height = height;
        _cc.center = center;
        _cc.radius = width / 2f;
    }

    void SetUpRigidbody()
    {
        if (!TryGetComponent(out _rb))
            _rb = gameObject.AddComponent<Rigidbody>();
        _rb.useGravity = true;
        _rb.isKinematic = false;
        _rb.constraints = RigidbodyConstraints.FreezeRotation;

        if (!TryGetComponent(out Collider collider))
        {
            collider = gameObject.AddComponent<CapsuleCollider>();
            collider.isTrigger = false;
            (collider as CapsuleCollider).radius = width / 2f;
            (collider as CapsuleCollider).height = height;
        }
    }

    private void Update()
    {
        if (!_input) return;
        // Ground check
        _isGrounded = Physics.CheckSphere(transform.position + Vector3.down * groundDistance, groundCheckerRadius,
            groundMask);

        if (controllerType == Controller.CharacterController)
        {
            CharacterControllerMovement();
        }
        else
        {
            _rb.drag = _isGrounded ? _groundDrag : 0f;
        }
        
        // Update state
        previousState = state;
        if (_isGrounded)
            if (IsMoving())
                state = Math.Abs(_currentSpeed - sprintSpeed) < 0.1f ? State.Running : State.Walking;
            else
                state = State.Idle;
        else
            state = State.Falling;
        
    }

    private void FixedUpdate()
    {
        if (!_input) return;
        if (controllerType == Controller.Rigidbody)
        {
            RigidbodyMovement();
        }
    }

    private void RigidbodyMovement()
    {
        // Movement
        if (!canAirControl && !_isGrounded) return;
        Vector3 movement = new Vector3(_input.Horizontal, 0, _input.Vertical);
        movement = transform.TransformDirection(movement);
        movement *= (_currentSpeed) * 10f;

        if (canAirControl && !_isGrounded) movement *= airControl;

        _rb.AddForce(movement);

        // Jump
        if (canJump && _input.Jump == ButtonState.Pressed || _input.Jump == ButtonState.Held && _isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void CharacterControllerMovement()
    {
        // Movement
        // Apply gravity
        if (_isGrounded && _velocity.y < 0)
            _velocity.y = -2f;
        else
            _velocity.y += Physics.gravity.y * Time.deltaTime;

        if (!canAirControl && !_isGrounded) return;
        Vector3 movement = new Vector3(_input.Horizontal, 0, _input.Vertical);
        movement = transform.TransformDirection(movement);
        movement *= _currentSpeed;

        if (canAirControl && !_isGrounded) movement *= airControl;

        _velocity.x = movement.x;
        _velocity.z = movement.z;

        // Jump
        if (canJump && _isGrounded && (_input.Jump == ButtonState.Pressed || _input.Jump == ButtonState.Held))
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * Physics.gravity.y);
        }

        _cc.Move(_velocity * Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * groundDistance, groundCheckerRadius);
    }

    public bool IsMoving()
    {
        Vector3 vel = controllerType == Controller.CharacterController ? _cc.velocity : _rb.velocity;
        return (new Vector2(vel.x, vel.z)).magnitude > 0.1f;
    }

    public override bool IsGrounded() => _isGrounded;
    
    public Vector3 GetVelocity() => controllerType == Controller.CharacterController ? _cc.velocity : _rb.velocity;
    
    public bool IsSprinting() => _currentSpeed == sprintSpeed;
    
    public override float GetSpeed() => _currentSpeed;
}
