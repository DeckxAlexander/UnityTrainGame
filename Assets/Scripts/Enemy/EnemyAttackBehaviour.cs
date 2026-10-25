using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
[RequireComponent(typeof(EnemyCharacter))]
public class EnemyAttackBehaviour : AttackBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    protected PlayerFeedback _colorChanger = null;

    protected Rigidbody _AttackerRigidbody;

    protected override void Start()
    {
        base.Start();
        _AttackerRigidbody = GetComponent<Rigidbody>();
        _colorChanger = FindFirstObjectByType<PlayerFeedback>();
    }



    protected override void AttackRaycast()
    {

        Vector3 rayDirection = _AttackerRigidbody.rotation * Vector3.forward;
        Debug.Log(_AttackerRigidbody.worldCenterOfMass);
        Debug.DrawRay(_AttackerRigidbody.worldCenterOfMass, rayDirection, Color.green, 5f);
        if (Physics.Raycast(_AttackerRigidbody.worldCenterOfMass, rayDirection, out RaycastHit hit, _range, _attackLayer))
        {
            HitTarget(hit);
            



        }
    }

    protected override void HitTarget(RaycastHit hit)
    {
        Health targetHealthComponent = hit.rigidbody.GetComponent<Health>();
        if (targetHealthComponent != null)
        {
            if (_playHitEffects && _colorChanger != null)
            {
                //PlayHitEffect(hit.point); //Currently not used for enemy attack
                _colorChanger.SetState(PlayerFeedback.VignetteState.Hit);
            }
            if (targetHealthComponent.Damage(_damage)) return; //return if object is killed
            if (_enableKnockback) ApplyKnockback(hit);
            

        }
    }

    protected override void ApplyKnockback(RaycastHit hit) //Navmesh check not included for enemy attack
    {
        Rigidbody targetRb = hit.rigidbody;
        if (targetRb == null) return;




        // Knockback direction = from attacker to target
        Vector3 knockbackDir = hit.transform.position - _attackerTransform.position;
        knockbackDir.y = 0f;

        targetRb.AddForce(knockbackDir.normalized * _knockbackForce, ForceMode.Impulse);
    }
}
