using UnityEngine;

public class BaseCharacter : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected CharacterMovement _characterMovement;
    protected AttackBehaviour _attackBehaviour;

    //Animations
    protected const string IDLE = "Idle";
    protected const string ATTACK = "Attack";
    protected string _currentState;
    protected Animator _animator;
    

    public void ChangeAnimationState(string state)
    {
        if (_currentState == state || _animator == null) return;

        _currentState = state;
        _animator.CrossFadeInFixedTime(_currentState, 0.2f);

    }

    protected void SetAnimation()
    {
        if (!_attackBehaviour.IsAttacking) ChangeAnimationState("Idle");


    }

    protected virtual void Awake() 
    {
        _characterMovement = GetComponent<CharacterMovement>();
        _attackBehaviour = GetComponent<AttackBehaviour>();



    }



}
