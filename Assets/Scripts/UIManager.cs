using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : BaseManager<UIManager>
{
    public GameObject mainUI;
    public GameObject dragUI;
    public GameObject playUI;
    public GameObject settingsUI;
    public GameObject pauseUI;
    public GameObject finishUI;
    public GameObject finishCoinsHolder;
    public GameObject finishNewBest;
    public GameObject finishScoreTitle;
    public GameObject loseUI;
    public GameObject shopUI;
    public GameObject tutorialUI;
    public GameObject tutorialTapTextObj;
    public GameObject offVibrationSettings;
    public GameObject offSoundSettings;
    public GameObject offVibrationPause;
    public GameObject offSoundPause;
    public GameObject creditsUI;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;
    public TextMeshProUGUI commonCoinsText;
    public TextMeshProUGUI playCoinsText;
    public TextMeshProUGUI finishCoinsText;
    public TextMeshProUGUI finishScoreText;
    private RectTransform _settingsRect;

    private delegate void Method();
    
    protected override void Awake()
    {
        if (!instance)
            instance = this;
        base.Awake();
    }

    protected override void InitializeManager()
    {
        //PlayerPrefs.DeleteKey("BestScore");
        SetScore(0);
        SetBestScore(PlayerPrefs.GetInt("BestScore", 0));
        SetCommonCoins();
        _settingsRect = settingsUI.GetComponent<RectTransform>();
    }

    public void StartGame()
    {
        mainUI.SetActive(false);
        playUI.SetActive(true);
    }

    public void SetFinish()
    {
        DragAndOneMoreObjOpenCreate(false, playUI);
        // if(finishCoinsText.text == "0")
        //     finishCoinsHolder.SetActive(false);
        // if (finishScoreText.text == "0")
        // {
        //     finishScoreTitle.SetActive(false);
        //     finishScoreText.gameObject.SetActive(false);
        // }
        Invoke(nameof(TrueFinish), 1.5f);
    }

    public void SetLose()
    {
        DragAndOneMoreObjOpenCreate(false, playUI);
        Invoke(nameof(TrueLose), 1.5f);
    }

    public void CreatePause()
    {
        DragAndOneMoreObjOpenCreate(false, playUI);
        pauseUI.SetActive(true);
    }

    public void KillPause()
    {
        DragAndOneMoreObjOpenCreate(true, playUI);
        pauseUI.SetActive(false);
    }

    public void CreateKillSettings() => StartCoroutine(DropSettings());

    public void CreateShop()
    {
        DragAndOneMoreObjOpenCreate(false, mainUI);
        shopUI.SetActive(true);
    }

    public void KillShop()
    {
        DragAndOneMoreObjOpenCreate(true, mainUI);
        shopUI.SetActive(false);
    }

    public void SwitchVibration(bool onVibration)
    {
        offVibrationSettings.SetActive(!onVibration);
        offVibrationPause.SetActive(!onVibration);
    }
    
    public void SwitchSound(bool onSound)
    {
        offSoundSettings.SetActive(!onSound);
        offSoundPause.SetActive(!onSound);
    }

    public void CreateTutorial()
    {
        tutorialUI.SetActive(true);
        StartCoroutine(InvokeRealTime(TrueTutorialTapTextObj, 2f));
    }

    public void KillTutorial() => tutorialUI.SetActive(false);

    public void CreateCredits() => creditsUI.SetActive(true);

    private void DragAndOneMoreObjOpenCreate(bool isCreate, GameObject obj)
    {
        dragUI.SetActive(isCreate);
        obj.SetActive(isCreate);
    }
    
    public void SetScore(int score) => scoreText.text = score.ToString();

    public void SetBestScore(int bestScore) => bestScoreText.text = "Best\n" + bestScore;

    public void SetPlayCoins(int coins) => playCoinsText.text = coins.ToString();

    public void SetCommonCoins() => commonCoinsText.text = PlayerPrefs.GetInt("Coins", 0).ToString();

    public void SetFinishCoins(int coins) => finishCoinsText.text = coins.ToString();

    public void SetFinishScore(int score) => finishScoreText.text = score.ToString();

    public void SetNewBest() => finishNewBest.SetActive(true);

    private void TrueFinish() => finishUI.SetActive(true);
    
    private void TrueLose() => loseUI.SetActive(true);

    private void TrueTutorialTapTextObj() => tutorialTapTextObj.SetActive(true);

    private IEnumerator DropSettings()
    {
        float offset = _settingsRect.sizeDelta.x == 210 ? -10f : 10f;
        float targetWidth = offset == -10f ? 0f : 210f;
        while (_settingsRect.sizeDelta.x != targetWidth)
        {
            _settingsRect.sizeDelta += new Vector2(offset,0f);
            yield return null;
        }
    }

    private IEnumerator InvokeRealTime(Method InvokedMethod, float delay)
    {
        float timeElapsed = 0f;
        while (timeElapsed < delay)
        {
            timeElapsed += Time.unscaledDeltaTime;
            yield return null;
        }
        InvokedMethod();
    }
}
