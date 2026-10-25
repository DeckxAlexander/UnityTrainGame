using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;

public class CharacterMovement : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] protected float _movementSpeed = 1.0f;
    [SerializeField] protected float _jumpStrength = 20f;
    [SerializeField] protected float _dashStrength = 20f;
    [SerializeField] protected float _dashDuration = 0.5f;
    [SerializeField] protected float _dashCooldown = 0.5f;
    [SerializeField] protected float _dashMovementMultiplier = 2f;
    


    protected Rigidbody _rigidbody;

    protected Vector3 _desiredMovementDirection = Vector3.zero;
    protected float _desiredRotation = -1f;
    protected bool _grounded = false;
    protected GameObject _target = null;
    protected const float GROUND_CHECK_DISTANCE = 0.7f;
    protected const string GROUND_LAYER = "Ground";






    private bool _canDash = true;
    private bool _isDashing = false;

    private const float _maxSlopeAngle = 60f;
    private RaycastHit _slopeHit;



    public float DashCooldown
    {
        get { return _dashCooldown; }
    }

    public bool IsCooldownActive
    {
        get { return !_canDash; }
    }

    public Vector3 DesiredMovementDirection
    {
        get { return _desiredMovementDirection; }
        set { _desiredMovementDirection = value; }
    }

    public float DesiredRotation
    {
        get { return _desiredRotation; }
        set { _desiredRotation = value; }
    }
    public GameObject Target
    {
        get { return _target; }
        set { _target = value; }
    }

    protected virtual void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _desiredRotation = transform.rotation.eulerAngles.y;
       
    }
    


    protected virtual void Update()
    {
        
        HandleRotation();
        SpeedControl();
        _grounded = Physics.Raycast(transform.position + Vector3.up * 0.3f, Vector3.down, GROUND_CHECK_DISTANCE, LayerMask.GetMask(GROUND_LAYER));
        if (_grounded)
        {
            _rigidbody.linearDamping = 2.5f;
        }
        else
        {
            _rigidbody.linearDamping = 0f;
        }

    }
    protected virtual void FixedUpdate()
    {
        HandleMovement();

    }

    protected virtual void HandleMovement()
    {
        if (_rigidbody == null) return;



        Vector3 movement = _desiredMovementDirection.normalized;
        movement *= _movementSpeed;


        if (OnSlope(movement.normalized)) 
        {
            //force the linear velocity when player is on a slope
            movement.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = movement;

        }
        else 
        {
            _rigidbody.AddForce(movement * 7f, ForceMode.Force);
        }

            
        _rigidbody.useGravity = !OnSlope(movement.normalized); //turn off gravity on slope
    }

    protected virtual void HandleRotation() 
    {
        gameObject.transform.rotation = Quaternion.Euler(0.0f, _desiredRotation, 0.0f);


    }
    public virtual void Jump() 
    {

        if ((_grounded) && !_isDashing) 
        { 
            _rigidbody.AddForce(Vector3.up * _jumpStrength, ForceMode.Impulse);


        }

    }


    public virtual void Dash() 
    {
        if (!_canDash) return;
        _canDash = false;
        _isDashing = true;
        Vector3 dashForce = _desiredMovementDirection.normalized * _dashStrength + Vector3.up * 5f;
        if (_desiredMovementDirection.normalized == Vector3.zero) dashForce = gameObject.transform.forward * _dashStrength + Vector3.up * 5f;
        //Debug.Log(m_DesiredMovementDirection);
        if (_grounded) _rigidbody.AddForce(dashForce, ForceMode.Impulse);
        _movementSpeed *= _dashMovementMultiplier;
        
        Invoke(nameof(EndDash), _dashDuration);
        Invoke(nameof(ResetDash), _dashCooldown);

    }

    private void EndDash() 
    {
        _movementSpeed /= _dashMovementMultiplier;
        _isDashing = false;
    }

    private void ResetDash() 
    {
        _canDash = true;
    }

   
    private void SpeedControl() 
    {
        Vector3 movementVelocity = new Vector3(_rigidbody.linearVelocity.x, 0f, _rigidbody.linearVelocity.z);
        if (movementVelocity.sqrMagnitude > _movementSpeed * _movementSpeed)
        {
            movementVelocity = movementVelocity.normalized * _movementSpeed;
            movementVelocity.y = _rigidbody.linearVelocity.y;
            _rigidbody.linearVelocity = movementVelocity;
        }




    }


    //Check if character is on a slope
    private bool OnSlope(Vector3 direction) 
    {
        if (direction == Vector3.zero) direction = Vector3.down; //if movement direction is zero, the direction will point down
        if (Physics.Raycast(transform.position + Vector3.up * 0.1f, direction, out _slopeHit, 0.5f, LayerMask.GetMask(GROUND_LAYER))) //check in front of the player if there is a slope
        {
            float angle = Vector3.Angle(Vector3.up, _slopeHit.normal);
            return angle < _maxSlopeAngle && angle != 0;

        }
        return false;
    }




}
