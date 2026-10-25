using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Serialization;
using static UnityEngine.GraphicsBuffer;


[RequireComponent(typeof(BaseCharacter))]
public class AttackBehaviour : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Header("Attack Settings")]

    [SerializeField] protected int _damage;
    [SerializeField] protected float _range;
    [SerializeField] protected float _cooldown;
    [SerializeField] protected float _delay = 0.4f;
    [SerializeField] protected float _speed = 2f;
    [SerializeField] protected LayerMask _attackLayer;
    [FormerlySerializedAs("m_AttackerTransform")]
    [SerializeField] protected Transform _attackerTransform;

    [Header("Knockback Settings")]
    [SerializeField] protected bool _enableKnockback = false;
    [SerializeField] protected float _knockbackForce = 5f;

    [Header("Effects")]
    [SerializeField] protected bool _playHitEffects = false;
    [SerializeField] protected GameObject _hitEffect;

    

    protected bool _attacking = false;
    protected float _lastAttackTime = -Mathf.Infinity;
    protected BaseCharacter _characterComponent;


    public bool IsAttacking {  get { return _attacking; } }

    public float GetAttackRange { get { return _range; } }

    protected virtual void Start()
    {
        _characterComponent = gameObject.GetComponent<BaseCharacter>();
    }

    public virtual void Attack() 
    {

        if (Time.time < _lastAttackTime + _cooldown || _attacking) return;
        _lastAttackTime = Time.time;
        _attacking = true;

        Invoke(nameof(ResetAttack), _speed); 
        Invoke(nameof(AttackRaycast), _delay); //Raycast after delay

        _characterComponent.ChangeAnimationState("Attack"); //Initiate Attack Animation
    }

    protected void ResetAttack() 
    {
        _attacking = false;

    }

    protected virtual void AttackRaycast() 
    {

        if (Physics.Raycast(_attackerTransform.position, _attackerTransform.forward, out RaycastHit hit, _range, _attackLayer)) 
        {
            
            HitTarget(hit);
            
        }
    }

    protected virtual void HitTarget(RaycastHit hit) 
    {
        Health targetHealthComponent = hit.rigidbody.GetComponent<Health>();
        if (targetHealthComponent != null)
        {
            if (_playHitEffects) PlayHitEffect(hit.point); //play cosmetic
            if (targetHealthComponent.Damage(_damage)) return; //damage and return if object is killed

            if (_enableKnockback) ApplyKnockback(hit); //apply knockback
            

        }
    }

    protected virtual void ApplyKnockback(RaycastHit hit)
    {
        Rigidbody targetRb = hit.rigidbody;
        if (targetRb == null) return;

        // Knockback direction = from attacker to target
        Vector3 knockbackDir = hit.transform.position - _attackerTransform.position;
        knockbackDir.y = 0f;

        //If target has navmeshAgent / enemy
        NavMeshAgent targetNavMeshAgent = targetRb.GetComponent<NavMeshAgent>();
        if (targetNavMeshAgent != null)
        {
            StartCoroutine(NavMeshKnockback(targetRb, knockbackDir, targetNavMeshAgent));
        }
        else 
        {
            // Knockback direction = from attacker to target
            targetRb.AddForce(knockbackDir.normalized * _knockbackForce, ForceMode.Impulse);
        }



            
            
    }

    protected void PlayHitEffect(Vector3 hit) 
    {
        if (_hitEffect == null) return;
        Instantiate(_hitEffect, hit, Quaternion.identity);
    }



    private IEnumerator NavMeshKnockback(Rigidbody targetRb, Vector3 knockbackDir, NavMeshAgent targetNavMeshAgent) 
    {
        targetNavMeshAgent.updatePosition = false;
        targetRb.isKinematic = false;
        targetRb.useGravity = true;



        targetRb.AddForce(knockbackDir.normalized * _knockbackForce, ForceMode.Impulse);

        yield return new WaitForFixedUpdate();
        yield return new WaitUntil(() => targetRb.linearVelocity.magnitude < 0.05f); //wait until it stops moving
        yield return new WaitForSeconds(0.25f);

        targetRb.linearVelocity = Vector3.zero;
        targetRb.angularVelocity = Vector3.zero;

        targetRb.isKinematic = true;
        targetRb.useGravity = false;
        targetNavMeshAgent.Warp(targetRb.transform.position);
        targetNavMeshAgent.updatePosition = true;

        yield return null;
    }
}
