using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
public class GameMenuScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _title;
    [SerializeField] private TextMeshProUGUI _timeText;

    [SerializeField] private Button _ResumeButton;
    [SerializeField] private Button _RestartButton;

    [Header("References")]
    [SerializeField] private GameController _gameController;


    private void Start()
    {
        if (_gameController == null) _gameController = FindFirstObjectByType<GameController>(); //Incase the serializefield is not set
    }

    public void SetupGameScreen(string title,float finalScore, float temporaryScore, float time ) 
    {
        gameObject.SetActive(true);
        _RestartButton.gameObject.SetActive(true);
        _title.text = title;
        _timeText.text = "Score: " + temporaryScore.ToString() + "\nTime: " + time.ToString() + "s\nFinalScore: " + finalScore.ToString();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void SetupPauseMenu(string title)
    {
        gameObject.SetActive(true);
        _ResumeButton.gameObject.SetActive(true);
        _title.text = title;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        
    }

    public void RestartButton() 
    {
        SceneManager.LoadScene(0);
    }

    public void QuitButton() 
    {
        Application.Quit();
    }

    public void ResumeButton() 
    {
        _gameController.TriggerResume();
        
        _ResumeButton.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        gameObject.SetActive(false);
    }
}
