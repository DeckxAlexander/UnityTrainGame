using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerFeedback : MonoBehaviour
{
    [Header("Vignette Color")]
    [SerializeField] private Volume _globalVolume = null;
    [SerializeField] private Color _inlightcolor = Color.white;
    [SerializeField] private Color _hitcolor = Color.white;
    [SerializeField] private Color _idlecolor = Color.black;
    [SerializeField] private float _hitEffectDuration = 0.5f;
    private Vignette _vignette = null;

    [Header("Sounds")]
    [SerializeField] private AudioSource _backgroundMusic = null;
    [SerializeField] private float _idleMusicVolume = 0.5f;
    [SerializeField] private AudioSource _PlayerHitSound = null;
    [SerializeField] private AudioSource _InLightSound = null;



    public enum VignetteState
    {
        Idle,
        Hit,
        InLight
    }

    private VignetteState _state = VignetteState.Idle;

    private void Start()
    {
        _globalVolume.profile.TryGet<Vignette>(out _vignette);
        if (_backgroundMusic != null) 
        {
            _backgroundMusic.Play();
            _backgroundMusic.volume = _idleMusicVolume;
        }

        if (_InLightSound != null)
        {
            _InLightSound.Play();
            _InLightSound.volume = 0f;
        }



    }



    public void SetState(VignetteState state) 
    {

        if (_state == VignetteState.Hit) return; //The hit effect has priority and will reset the state when finished

        switch (state)
        {
            case VignetteState.Idle:
                _state = state;
                if (_backgroundMusic != null) _backgroundMusic.volume = _idleMusicVolume;
                if (_InLightSound != null) _InLightSound.volume = 0f;

                SetColor(_idlecolor);
                SetIntensity(0.45f);

                break;

            case VignetteState.Hit:
                HitEffect();
                break;

            case VignetteState.InLight:
                _state = state;

                SetColor(_inlightcolor);
                SetIntensity(0.8f);
                if (_backgroundMusic != null) _backgroundMusic.volume = 0.7f;
                if (_InLightSound != null) _InLightSound.volume = 0.6f;

                break;

        }
    }



    private void HitEffect() 
    {


        _PlayerHitSound?.Play();
        _state = VignetteState.Hit;


        SetColor(_hitcolor);
        SetIntensity(0.6f);

        Invoke(nameof(SetIdle), _hitEffectDuration);

    }


    private void SetIdle() 
    {
        
        _state = VignetteState.Idle;
        
        SetState(VignetteState.Idle);
        

    }



    private void SetColor(Color color) 
    {
        _vignette.color.value = color;
    }
    private void SetIntensity(float intensity) 
    {
        _vignette.intensity.value = intensity;
    }


}
