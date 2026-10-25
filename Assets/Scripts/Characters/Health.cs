using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

public class Health : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private int _startHealth = 5;

    [Header("On damaged")]
    [SerializeField] private AudioClip _hitSound = null;

    [Header("On Death")]
    [SerializeField] private UnityEvent _onDeath;




    protected int _health;

    public int CurrentHealth 
    {
        get { return _health; }
    }

    public int StartHealth
    {
        get { return _startHealth; }
    }

    private void Awake()
    {
        _health = _startHealth;
    }
    private void Start()
    {

        //if (_playSound) _audioManager = FindFirstObjectByType<AudioManager>();
    }

    public virtual bool Damage(int damage) 
    {

        _health -= damage;

        //_audioManager?.PlayHitSoundAtPosition(transform.position);
        if(_hitSound != null) AudioSource.PlayClipAtPoint(_hitSound, transform.position);

        //returns true if object is killed
        if (_health <= 0) {
            Kill(); 

            return true;
        }
        return false;

        

    }

    public virtual void Heal(int amount) 
    {
        if (_health + amount <= _startHealth )_health += amount;
       

    }

    public virtual void Kill() 
    {
        
        Destroy(gameObject);
        _onDeath?.Invoke();



    }
}
