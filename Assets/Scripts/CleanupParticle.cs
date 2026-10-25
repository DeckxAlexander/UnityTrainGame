using UnityEngine;

public class CleanupParticle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private float _lifeTime = 0.5f;

    private void Awake()
    {
        Invoke(nameof(Kill), _lifeTime);
    }

    private void Kill() 
    {
        Destroy(gameObject);
    }
}
