using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(PlayerCharacter))]
public class PlayerHealth : Health
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private Camera _camera = null;
    void Start()
    {
        _camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    public override void Kill()
    {
        if (_camera != null) _camera.transform.parent = null; //detach camera on kill
        base.Kill();    
    }
}
