using UnityEngine;

public class MovingObject : MonoBehaviour
{
    [SerializeField]private float _speed = 20f;
    
    [SerializeField] private float _lifetime = 10f;
    private float _spawnTime;

    private void Awake()
    {
        _spawnTime = Time.time;
    }



    private void Update()
    {
        transform.position += transform.forward * _speed * Time.deltaTime;

        //Destroy after lifetime expires
        if (Time.time >= _spawnTime + _lifetime)
        {
            Destroy(gameObject);
        }
    }
}
