using UnityEngine;

public class EndZone : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] private GameController _gameController;

    private void OnTriggerEnter(Collider other)
    {


        if (!other.CompareTag("Player")) return; //return if not player
        _gameController.TriggerWin();
        Destroy(gameObject);


    }

}
