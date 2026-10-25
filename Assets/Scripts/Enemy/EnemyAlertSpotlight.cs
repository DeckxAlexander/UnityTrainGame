using Unity.VisualScripting;
using UnityEngine;

public class EnemyAlertSpotlight : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Alert Spotlight Config")]
    [SerializeField] private float _range = 10f;
    [SerializeField] private float _cooldown = 20f;
    [SerializeField] private float _duration = 3f;
    [SerializeField] private GameObject _spotlightEnemy;
    [SerializeField] private bool _playEffects = true;
    [SerializeField] protected GameObject _effect;



    private EnemySpotlight _spotlightScript;
    private GameObject _player;
    private float _lastAlertTime = -Mathf.Infinity;
    private Vector3 _effectPosition = Vector3.zero;


    private void Start()
    {
        _spotlightScript = _spotlightEnemy.GetComponent<EnemySpotlight>();
        _player = FindFirstObjectByType<PlayerCharacter>().gameObject;
        
    }



    public void TryAlert() 
    {
        if (_player == null 
            ||Time.time < _lastAlertTime + _cooldown || //cooldown check
            (transform.position - _player.transform.position).sqrMagnitude > _range * _range ) return; //if not in range return

        _lastAlertTime = Time.time;
        _spotlightScript.ForceTarget(_player.transform, _duration);
        Debug.Log("Alert");
        if (_playEffects) PlayEffects();
        
    }

    private void PlayEffects()
    {
        if (_effect == null) return;
        _effectPosition = transform.position;
        _effectPosition.y += 1;
        Instantiate(_effect, _effectPosition, Quaternion.identity);
    }






}
