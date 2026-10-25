using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCharacter : BaseCharacter
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created


    [Header ("Input")]
    [SerializeField]
    private InputActionAsset _inputAsset;

    [SerializeField]
    private InputActionReference _movementAction;

    [SerializeField]
    private InputActionReference _lookAction;

    [SerializeField]
    private InputActionReference _jumpAction;

    [SerializeField]
    private InputActionReference _dashAction;

    [SerializeField]
    private InputActionReference _attackAction;

    [Header("Camera")]
    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private float _mouseSensitivity;

    private Vector2 _lookAt = Vector2.zero;

    private PickupBehaviour _pickupBehaviour;








    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;


        _animator = GetComponentInChildren<Animator>();
        _pickupBehaviour = GetComponentInChildren<PickupBehaviour>();



    }

    protected override void Awake()
    {
        base.Awake();
        
        //Attach input to methods
        _jumpAction.action.performed += HandleJumpInput;
        _attackAction.action.performed += HandleAttackInput;
        _dashAction.action.performed += HandleDashInput;

        _lookAt.x = transform.rotation.eulerAngles.y;
    }


    private void OnEnable()
    {
        if (_inputAsset == null) return;
        _inputAsset.Enable();
    }

    private void OnDisable()
    {
        if (_inputAsset == null) return;
        _inputAsset.Disable();
    }

    // Update is called once per frame
    private void Update()
    {
        HandleMovementInput();
        HandleRotationInput();
        SetAnimation();

    }



    private void HandleMovementInput()
    {
        if (_characterMovement == null || _movementAction == null) return;
        Vector2 movementInput = _movementAction.action.ReadValue<Vector2>();
        Vector3 movement = movementInput.y * transform.forward + movementInput.x * transform.right;

        _characterMovement.DesiredMovementDirection = movement; //sends the movement direction to the charactermovement script
    }

    private void HandleRotationInput()
    {
        Vector2 input = _lookAction.action.ReadValue<Vector2>();
        input *= _mouseSensitivity;
        _lookAt += input;
        _lookAt.y = Mathf.Clamp(_lookAt.y, -89.0f, 89.0f);
        _camera.transform.localRotation = Quaternion.Euler(-_lookAt.y, 0.0f, 0.0f); //handles the y rotation
        _characterMovement.DesiredRotation = _lookAt.x; //sends the X rotation to the character movement script
    }


    private void HandleJumpInput(InputAction.CallbackContext context) 
    {
        if (_characterMovement == null) return;
        _characterMovement.Jump();
    }

    private void HandleDashInput(InputAction.CallbackContext context)
    {
        if (_characterMovement == null) return;
        _characterMovement.Dash();
    }

    private void HandleAttackInput(InputAction.CallbackContext context) 
    {
        if (_attackBehaviour == null) return;
        _attackBehaviour.Attack();
    }
    



}
