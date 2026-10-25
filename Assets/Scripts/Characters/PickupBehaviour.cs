using UnityEngine;
using UnityEngine.InputSystem;

public class PickupBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("References")]
    [SerializeField] private Camera _playerCamera;
    [SerializeField] private Transform _mirrorSocket;

    [Header("Settings")]
    [SerializeField] private float _pickupRange = 3f;
    [SerializeField] private string _mirrorTag = "Mirror";
    [SerializeField] private InputActionReference _interactAction;
    [SerializeField] private InputActionReference _dropAction;

    private GameObject equippedMirror;

    public bool HasMirror 
    {
        get { return equippedMirror != null; }
    }

    private void Awake()
    {
        _interactAction.action.performed += HandleInteractInput;
        _dropAction.action.performed += HandleDropInput;
    }

    private void HandleInteractInput(InputAction.CallbackContext context) 
    {
        if (_playerCamera == null) return;

        Ray ray = new Ray(_playerCamera.transform.position, _playerCamera.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, _pickupRange))
        {
            if (hit.collider.CompareTag(_mirrorTag))
            {
                PickupMirror(hit.collider.gameObject);
            }
        }

    }

    private void HandleDropInput(InputAction.CallbackContext context)
    {
        if (equippedMirror == null) return;
        DropMirror();
    }

    private void PickupMirror(GameObject mirror)
    {
        equippedMirror = mirror;

        // Disable physics
        if (mirror.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = true;
            rb.detectCollisions = false;
        }

        // Disable collider to prevent clipping
        if (mirror.TryGetComponent(out Collider col))
        {
            col.enabled = false;
        }

        // Attach to socket
        mirror.transform.SetParent(_mirrorSocket);
        mirror.transform.localPosition = Vector3.zero;
        mirror.transform.localRotation = Quaternion.identity;
    }

    private void DropMirror()
    {


        // Detach
        equippedMirror.transform.SetParent(null);

        // Re-enable physics
        if (equippedMirror.TryGetComponent(out Rigidbody rb))
        {
            rb.isKinematic = false;
            rb.detectCollisions = true;
            rb.AddForce(_playerCamera.transform.forward * 3f, ForceMode.Impulse);
        }

        if (equippedMirror.TryGetComponent(out Collider col))
        {
            col.enabled = true;
        }

        equippedMirror = null;
    }

    public void BreakMirror()
    {

        if (equippedMirror == null) return;
        Destroy(equippedMirror);
        equippedMirror = null;
    }
}
