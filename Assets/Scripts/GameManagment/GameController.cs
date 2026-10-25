using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

public class GameController : MonoBehaviour
{

    [Header("References")]
    [SerializeField] private GameObject _player = null;
    [SerializeField] private GameMenuScreen _gameScreen = null;
    [SerializeField] private ScoreSystem _scoreSystem = null;

    [Header("Input")]
    [SerializeField]
    private InputActionAsset _inputAsset;
    [SerializeField] private InputActionReference _pauseAction;

    private bool _isEnded = false;
    private bool _isPaused = false;

    private void OnEnable()
    {
        if (_inputAsset == null) return;
        _inputAsset.Enable();
    }

    private void OnDisable()
    {
        if (_inputAsset == null) return;
        _inputAsset.Disable();
    }

    private void Start()
    {
        _pauseAction.action.performed += HandlePauseInput;
    }

    private void Update()
    {
        if ((_player == null || _player.transform.position.y < -20) && !_isEnded) TriggerGameOver();

    }
    private void TriggerGameOver() 
    {
        _isEnded = true;
        float finalScore = 0f;
        float temporaryScore = 0f;
        if (_player != null) _player.GetComponent<Health>().Kill(); //If the player is not killed yet
        if (_scoreSystem != null) 
        {
            finalScore = _scoreSystem.CalculateFinalScore(Time.time, 0f);
            temporaryScore = _scoreSystem.TemporaryScore;
        }
            
            
        _gameScreen.SetupGameScreen("GameOver", finalScore, temporaryScore, Mathf.Round(Time.time));

    }

    public void TriggerWin() 
    {
        _isEnded = true;
        float finalScore = 0f;
        float temporaryScore = 0f;
        if (_scoreSystem != null)
        {
            finalScore = _scoreSystem.CalculateFinalScore(Time.time, 2000f);
            temporaryScore = _scoreSystem.TemporaryScore;
        }


        _gameScreen.SetupGameScreen("GameOver", finalScore, temporaryScore + 2000f, Mathf.Round(Time.time));
        _player.GetComponent<PlayerHealth>()?.Kill(); //Destroys the player

    }

    public void TriggerResume() 
    {
        Time.timeScale = 1f; //resumes the game 
        _player.GetComponent<PlayerCharacter>().enabled = true; //enables all player input
        _isPaused = false;
    }
    private void TriggerPause() 
    {
        if (Time.timeScale == 0f) //If the game is already paused
        { 
            return; 
        } 
        _gameScreen.SetupPauseMenu("Pause");
        Time.timeScale = 0f; //pauses the game
        _player.GetComponent<PlayerCharacter>().enabled = false; //Disables all player input
        _isPaused = true;


    }

    private void HandlePauseInput(InputAction.CallbackContext context) 
    {
        if (_isEnded) return; 
        if (_isPaused) //If the game is already paused
        {
            _gameScreen.ResumeButton(); //Force the game to resume via the gamescreen without the button being pushed!
            return;
        }
        {
            TriggerPause();
            return;
        }

    }


}
