using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;

[RequireComponent(typeof(NavMeshAgent))]
public class NavMeshMovement : CharacterMovement
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private NavMeshAgent _navMeshAgent;
    private Vector3 _previousTargetPosition = Vector3.zero;
    private Vector3 _targetDirection = Vector3.zero;

    [SerializeField] private float _detectionRange = 5f;


    protected override void Awake()
    {
        base.Awake();

        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.speed = _movementSpeed;
        _navMeshAgent.updateRotation = false;



        _previousTargetPosition = transform.position;

    }

    protected override void Update()
    {
        HandleRotation();
    }

        const float MOVEMENT_EPSILON = 0.3f;
    protected override void HandleMovement()
    {

        if (_target == null)
        {
            _navMeshAgent.isStopped = true;
            return;
        }

        //should the target move we should recalculate our path
        if ((_target.transform.position - _previousTargetPosition).sqrMagnitude
            > MOVEMENT_EPSILON && IsTargetWithinDistance())
        {
            _navMeshAgent.SetDestination(_target.transform.position);
            _navMeshAgent.isStopped = false;
            _previousTargetPosition = _target.transform.position;




        }





    }


    protected override void HandleRotation()
    {
        if (_target == null) return;


        _targetDirection = _target.transform.position - transform.position;
        _targetDirection.y = 0f;


        if (_targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(_targetDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _navMeshAgent.angularSpeed);
        }

    }

    private bool IsTargetWithinDistance()
    {
        float distance = Vector3.Distance(transform.position, _target.transform.position);
        return distance <= _detectionRange;
    }
}