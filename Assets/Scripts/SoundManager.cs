using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : BaseManager<SoundManager>
{
    public GameObject finishSoundPref;
    public GameObject loseSoundPref;
    public GameObject juiceSoundPref;
    public GameObject cutSoundPref;
    public GameObject bombSoundPref;
    public GameObject coinCutSoundPref;
    public AudioSource stepSoundAudio;
    private UIManager _uiManager;
    private GameManager _gm;
    private Camera _camera;
    
    protected override void Awake()
    {
        if (!instance)
            instance = this;
        base.Awake();
    }

    private void Start()
    {
        _uiManager = UIManager.instance;
        _uiManager.SwitchVibration(IsOnVibration());
        _uiManager.SwitchSound(IsOnSound());
        _gm = GameManager.instance;
        _camera = Camera.main;
    }

    public void SwitchVibration()
    {
        int counter = IsOnVibration() ? 0 : 1;
        PlayerPrefs.SetInt("Vibration", counter);
        _uiManager.SwitchVibration(IsOnVibration());
    }
    
    public void SwitchSound()
    {
        int counter = IsOnSound() ? 0 : 1;
        PlayerPrefs.SetInt("Sound", counter);
        _uiManager.SwitchSound(IsOnSound());
    }

    public bool IsOnVibration() => PlayerPrefs.GetInt("Vibration", 1) != 0;
    
    public bool IsOnSound() => PlayerPrefs.GetInt("Sound", 1) != 0;
    
    public void CreateFinishSound() => CreateSoundPref(finishSoundPref, 4f);
    
    public void CreateLoseSound() => CreateSoundPref(loseSoundPref, 3f);
    
    public void CreateStepSound() => ReadySoundPlay(stepSoundAudio);
    
    public void CreateJuiceSound() => CreateSoundPref(juiceSoundPref, 2f);
    
    public void CreateCutSound() => CreateSoundPref(cutSoundPref, 2f);
    
    public void CreateBombSound() => CreateSoundPref(bombSoundPref, 3f);
    
    public void CreateCoinCutSound() => CreateSoundPref(coinCutSoundPref, 2f);
    
    private void CreateSoundPref(GameObject soundPref, float destroyDelay)
    {
        if(PlayerPrefs.GetInt("Sound", 1) == 0 || !soundPref)
            return;
        
        GameObject soundObj = GameObject.Instantiate(soundPref);
        soundObj.transform.SetParent(_camera.transform);
        soundObj.GetComponent<AudioSource>().Play();
        Destroy(soundObj, destroyDelay);
    }

    private void ReadySoundPlay(AudioSource sound)
    {
        if(PlayerPrefs.GetInt("Sound", 1) == 0 || !sound)
            return;
        
        sound.Play();
    }

    public void CreateStepSoundInTime(float startTime, float repeatTime, bool condition)
    {
        if(!condition)
            return;
        
        InvokeRepeating(nameof(CreateStepSound), startTime, repeatTime);
    }

    public void KillStepSound() => CancelInvoke(nameof(CreateStepSound));
}
