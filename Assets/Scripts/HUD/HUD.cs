using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Image _healthBarImage;
    [SerializeField] private Image _CooldownBarImage;
    [SerializeField] private TextMeshProUGUI _temporaryScoreText;
    [SerializeField] private ScoreSystem _scoreSystem;

    private Health _playerHealth;
    private CharacterMovement _characterMovement;
    private float _cooldownTimer;
    private bool _isCalculatingCooldown;

    private void Start()
    {
        PlayerCharacter playerCharacter = FindFirstObjectByType<PlayerCharacter>();
        _playerHealth = playerCharacter.GetComponent<Health>();
        _characterMovement = playerCharacter.GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        UpdateHealthBar();
        UpdateCooldownBar();
        UpdateTemporaryScore();
    }

    private void UpdateHealthBar() 
    {

        if (_playerHealth == null) SetBarPercentage(_healthBarImage, 0f, 100f);
        else SetBarPercentage(_healthBarImage, _playerHealth.CurrentHealth, _playerHealth.StartHealth);
    }

    private void UpdateCooldownBar() 
    {
        if (_characterMovement != null) 
        {
            if (_characterMovement.IsCooldownActive) 
            {
                if (!_isCalculatingCooldown) 
                {
                    _isCalculatingCooldown = true;
                    _cooldownTimer = _characterMovement.DashCooldown;
                }
                _cooldownTimer -= Time.deltaTime;
                SetBarPercentage(_CooldownBarImage, _cooldownTimer, _characterMovement.DashCooldown);
                return;
                

            }
            else 
            {
                _cooldownTimer = 0f;
                _isCalculatingCooldown = false;
            }

        }
        //If the cooldown is not active
        SetBarPercentage(_CooldownBarImage, 0f, 100f);
        

            

    }
    
    private void UpdateTemporaryScore() 
    {
        if (_temporaryScoreText != null && _playerHealth != null && _scoreSystem != null) _temporaryScoreText.text = _scoreSystem.TemporaryScore.ToString();
        else _temporaryScoreText.text = string.Empty;
    }

    private void SetBarPercentage(Image bar, float health, float max) 
    {
        if (bar == null) return;
        float percentage = health / max;

        bar.transform.localScale = new Vector3(percentage, 1f, 1f);
        
    }


}
