using UnityEngine;

public class EnemyCharacter : BaseCharacter
{
    

    [Header("Enemy Configuration")]
    [SerializeField] private bool _alertLight = false;

    private GameObject _playerTarget = null;
    private float _attackRange;
    private EnemyAlertSpotlight _alertSpotlight;




    private void Start()
    {
        PlayerCharacter player = FindFirstObjectByType<PlayerCharacter>();

        if (player != null) _playerTarget = player.gameObject;
        if (_attackBehaviour != null ) _attackRange = _attackBehaviour.GetAttackRange;
        if (_alertLight) _alertSpotlight = GetComponent<EnemyAlertSpotlight>();
        _animator = GetComponentInChildren<Animator>();
        
    }

    private void Update()
    {
        HandleMovement();
        HandleAttacking();
        SetAnimation();
        if (_alertLight) HandleSpotlightAlert();
    }

    void HandleMovement()
    {
        if (_characterMovement == null)
            return;

        _characterMovement.Target = _playerTarget;
    }

    void HandleAttacking()
    {
        if (_attackBehaviour == null) return;

        if (_playerTarget == null) return;

        if ((transform.position - _playerTarget.transform.position).sqrMagnitude
            < _attackRange * _attackRange)
        {
            _attackBehaviour.Attack();

        }
    }

    private void HandleSpotlightAlert() 
    {
        _alertSpotlight.TryAlert();
    }



}
