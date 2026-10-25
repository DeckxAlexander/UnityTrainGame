using NUnit.Framework;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;

public class EnemySpotlight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Light")]
    [SerializeField] private Light _spotLight;
    [SerializeField] private Light _pointLight;
    [SerializeField] private List<GameObject> _checkpoints;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _disableCooldown = 10f;
    [SerializeField] private float _damageCooldown = 1f;


    [Header("Player")]
    [SerializeField] private float _aimAngleOffset = 10f;
    [SerializeField] private Rigidbody _playerRigidBody;

    [Header("Feedback")]
    [SerializeField] private GameObject _playerFeedback;
    [SerializeField] private AudioSource _shutdownSound;


    private Camera _playerCamera;
    private bool _seesPlayer = false;
    private bool _lightEnabled = true;
    
    private int _targetCheckpointIndex = 0;
    private float _disabledTime = -Mathf.Infinity;

    private Health _playerHealthComponent;
    private PickupBehaviour _playerPickupComponent;
    private float _timepassed = 0f;

    private PlayerFeedback _colorChanger;

    private bool _ignoreCheckpoints = false;
    private Transform _forcedTarget = null;
    private float _forcedTargetDuration = 0f;

    private void Start()
    {
        _playerHealthComponent = _playerRigidBody.GetComponent<Health>();
        _playerPickupComponent = _playerRigidBody.GetComponent<PickupBehaviour>();
        _playerCamera = _playerRigidBody.GetComponentInChildren<Camera>();
        _colorChanger = _playerFeedback.GetComponent<PlayerFeedback>();
    }

    void Update()
    {
        if (_lightEnabled) 
        {
            SpotlightDamage();
            UpdateSpotlightPosition();
        }
        else TryEnableSpotlight();

        


    }

    private bool IsPlayerAimingMirror() 
    {
        if (!_playerPickupComponent.HasMirror || _playerCamera == null) return false;
        Vector3 toTarget = _spotLight.transform.position - _playerRigidBody.worldCenterOfMass;
        float angleToTarget = Vector3.Angle(_playerCamera.transform.forward, toTarget);
        return angleToTarget < _aimAngleOffset;

    }

    private void DisableSpotlight() 
    {

        _shutdownSound?.Play();

        _lightEnabled = false;
        _disabledTime = Time.time;

        _spotLight.enabled = false;
        _pointLight.enabled = false;

        _colorChanger.SetState(PlayerFeedback.VignetteState.Idle);
        Debug.Log("Broken");

    }

    private void TryEnableSpotlight() 
    {
        if (Time.time < _disabledTime + _disableCooldown) return;

        _lightEnabled = true;
        _disabledTime = -Mathf.Infinity;

        _spotLight.enabled = true;
        _pointLight.enabled = true;
    }


    private void SpotlightDamage() 
    {
        if (_playerHealthComponent == null) return;
        if (IsSpotlightTouchingPlayer())
        {
            _seesPlayer = true;
            _timepassed += Time.deltaTime;
            _colorChanger.SetState(PlayerFeedback.VignetteState.InLight);

            if (_playerPickupComponent.HasMirror && IsPlayerAimingMirror()) 
            {
                DisableSpotlight();
                _playerPickupComponent.BreakMirror();


            }
            else 
            {
                if (_timepassed >= _damageCooldown)
                {

                    _playerHealthComponent.Damage(1);
                    
                    _timepassed = 0f;

                }
            }
        }
        else
        {
            _timepassed = 0f;
            _seesPlayer = false;
            _colorChanger.SetState(PlayerFeedback.VignetteState.Idle);

        }

        
    }

    private bool IsSpotlightTouchingPlayer() 
    {
        if (_spotLight == null && _playerRigidBody == null) return false;

        

        Vector3 toTarget = _playerRigidBody.worldCenterOfMass - _spotLight.transform.position;
        Vector3 directionToTarget = toTarget.normalized;
        float distanceToTarget = toTarget.magnitude;
        float angleToTarget = Vector3.Angle(_spotLight.transform.forward, toTarget);

        if (angleToTarget < _spotLight.spotAngle / 2f) 
        {
            Debug.DrawLine(_spotLight.transform.position, _playerRigidBody.worldCenterOfMass);
            if (Physics.Raycast(_spotLight.transform.position, directionToTarget, out RaycastHit hit)) 
            {
                if (hit.rigidbody == _playerRigidBody) 
                {
                    return true;
                }
            }
            

        }

        return false;




    }

    private void UpdateSpotlightPosition() 
    {
        if (_playerRigidBody == null) return;

        if (_seesPlayer)
        {
            AimToTarget(_playerRigidBody.transform, _rotationSpeed * 2f);
        }
        else if (!_ignoreCheckpoints)
        {
            if (_checkpoints.Count > 0)
            {
                if (AimToTarget(_checkpoints[_targetCheckpointIndex].transform, _rotationSpeed))
                {
                    //If rotation complete
                    _targetCheckpointIndex = (_targetCheckpointIndex + 1) % _checkpoints.Count;
                }
            }
        }
        else if (_forcedTarget != null) ForceAimtToTarget(_forcedTarget, _rotationSpeed); //When alerted by enemy





    }
    private bool AimToTarget(Transform target, float speed) 
    {
        if (target != null)
        {
            Vector3 direction = target.position - _spotLight.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _spotLight.transform.rotation = Quaternion.Lerp(_spotLight.transform.rotation, targetRotation, Time.deltaTime * speed);
            float angle = Quaternion.Angle(_spotLight.transform.rotation, targetRotation);
            if (angle < 5f)
            {
                return true; //Rotation complete
            }
        }
        return false; 
    }

    public void ForceTarget(Transform target, float duration = 10f) //When alerted by enemy
    {
        if (target == null) return;
        

        _forcedTarget = target;
        _ignoreCheckpoints = true;
        _forcedTargetDuration = duration;




    }

    private bool ForceAimtToTarget(Transform target, float speed) 
    {
        

            Vector3 direction = target.position - _spotLight.transform.position;

            Quaternion targetRotation = Quaternion.LookRotation(direction);

            _spotLight.transform.rotation = Quaternion.Lerp(_spotLight.transform.rotation, targetRotation, Time.deltaTime * speed);
            float angle = Quaternion.Angle(_spotLight.transform.rotation, targetRotation);
            if (angle < 5f)
            {
                Invoke(nameof(ResumeCheckpointChecks), _forcedTargetDuration);
                _forcedTarget = null;
                return true; //Rotation complete
            }
        return false;
    }

    private void ResumeCheckpointChecks() 
    {
        _ignoreCheckpoints = false;
    }



}
