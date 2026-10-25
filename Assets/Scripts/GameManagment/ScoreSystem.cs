using UnityEngine;

public class ScoreSystem : MonoBehaviour
{
    [Header("Score System Config")]
    [SerializeField] private float _pointsPerKill = 300f;
    [SerializeField] private float _timeFactor = 400f;

    private float _TemporaryScore = 0f;

    public float TemporaryScore { get { return _TemporaryScore; } }









    private void Start()
    {

    
    }

    public float CalculateFinalScore(float timeSeconds, float baseScore ) 
    {

        float result = (baseScore + _TemporaryScore) * (_timeFactor / (timeSeconds + _timeFactor))* (_timeFactor / (timeSeconds + _timeFactor));
        
        return Mathf.Round(result);

    }

    public void IncrementTemporaryScore() 
    {
        _TemporaryScore += _pointsPerKill;
    }


}
