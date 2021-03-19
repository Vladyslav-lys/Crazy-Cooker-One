using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : BaseManager<GameManager>
{
    public bool isTutorial;
    public bool debugLevel;
    public int level;
    public int score;
    public int bestScore;
    public int coins;
    public bool isStarted;
    public bool isFinished;
    public bool isLosing;
    public GameObject[] levels;
    private PlayerMovement _playerMovement;
    private SpawnManager _sm;
    private UIManager _uiManager;
    private SoundManager _soundManager;
    private TargetCamera _targetCamera;
    
    protected override void Awake()
    {
        if (!instance)
            instance = this;
        base.Awake();
    }

    protected override void InitializeManager()
    {
        score = 0;
        coins = 0;
        bestScore = PlayerPrefs.GetInt("BestScore", 0);
        if(debugLevel)
            PlayerPrefs.SetInt("Level",level);
        level = PlayerPrefs.GetInt("Level", 1);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        _sm = SpawnManager.instance;
        _playerMovement = PlayerMovement.instance;
        _uiManager = UIManager.instance;
        _soundManager = SoundManager.instance;
        _targetCamera = Camera.main.GetComponent<TargetCamera>();
        levels[level-1].SetActive(true);
    }

    public void StartGame()
    {
        if(isStarted && !isFinished)
            return;
        
        isStarted = true;
        _playerMovement.playerAnimator.SetBool("Running", true);
        _uiManager.StartGame();
        _soundManager.CreateStepSoundInTime(0f, 0.35f, !isFinished && isStarted);
        CreateTutorial();
        //_sm.SpawnInTime(2.5f);
    }

    public void SetFinish()
    {
        isFinished = true;
        _soundManager.CreateFinishSound();
        _soundManager.KillStepSound();
        _playerMovement.StopPlayer();
        _playerMovement.playerAnimator.SetBool("Dancing", true);
        _uiManager.SetFinishCoins(coins);
        _uiManager.SetFinishScore(score);
        _uiManager.SetFinish();
        _targetCamera.enabled = true;
        SetBestScore();
        SetCommonCoins();
    }

    public void SetLose()
    {
        if(_soundManager.IsOnVibration())
            Handheld.Vibrate();
        isStarted = false;
        isLosing = true;
        _playerMovement.StopPlayer();
        _uiManager.SetLose();
        _soundManager.KillStepSound();
    }

    public void CreatePause()
    {
        _uiManager.CreatePause();
        Time.timeScale = 0f;
    }

    public void KillPause()
    {
        _uiManager.KillPause();
        Time.timeScale = 1f;
    }

    public void SetScore() => _uiManager.SetScore(++score);

    public void SetBestScore()
    {
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("BestScore",score);
            _uiManager.SetBestScore(score);
            _uiManager.SetNewBest();
        }
    }

    public void SetPlayCoins() => _uiManager.SetPlayCoins(++coins);

    public void SetCommonCoins() => PlayerPrefs.SetInt("Coins", PlayerPrefs.GetInt("Coins", 0) + coins);
    
    public void StartNextLevel()
    {
        if (level == levels.Length)
            level = 0;
        PlayerPrefs.SetInt("Level", ++level);
        RestartLevel();
    }

    public void RestartLevel() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    public void CreateTutorial()
    {
        if(PlayerPrefs.GetInt("IsTutorial",1) != 1 && !isTutorial)
            return;
        _uiManager.CreateTutorial();
        Time.timeScale = 0f;
    }

    public void KillTutorial()
    {
        PlayerPrefs.SetInt("IsTutorial",0);
        Time.timeScale = 1f;
        _uiManager.KillTutorial();
    }
}
