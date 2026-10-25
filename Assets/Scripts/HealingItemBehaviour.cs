using UnityEngine;

public class HealingItemBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int _healingAmount = 5;
      
    [SerializeField] private GameObject _mesh = null;
    [SerializeField] private float _rotationSpeed = 10f;
    private Vector3 _rotationVector = Vector3.zero;

    private void Awake()
    {
        _rotationVector = new Vector3(0, _rotationSpeed, 0);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RotateMesh();
    }

    private void OnTriggerEnter(Collider other)
    {


        if (!other.CompareTag("Player")) return; //return if not player

        Health healthComponent = other.GetComponent<Health>();

        if (healthComponent != null ) healthComponent.Heal(_healingAmount);
        Destroy(gameObject);
        
    }

    private void RotateMesh() 
    {
        if (_mesh == null) return;
        
        _mesh.transform.Rotate(_rotationVector * Time.deltaTime);
    }
}
