using UnityEngine;

public class WorldScroller : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [Header("Spawn Settings")]
    [SerializeField] private GameObject _objectPrefab;  
    [SerializeField] private float _spawnInterval = 2f; 
    [SerializeField] private float _spawnDistance = 10f;
    [SerializeField] private float _offset = 20f;

          
    

    private float m_LastSpawnTime = 0f;

    private void Update()
    {
        if (Time.time >= m_LastSpawnTime + _spawnInterval)
        {
            _offset = -_offset;
            SpawnObject();
            m_LastSpawnTime = Time.time;

        }
    }

    private void SpawnObject()
    {
        if (_objectPrefab == null)
        {
            return;
        }

        Vector3 spawnPos = transform.position + transform.forward * -_spawnDistance + transform.right * _offset;
        Instantiate(_objectPrefab, spawnPos, transform.rotation);

        
    }
}
