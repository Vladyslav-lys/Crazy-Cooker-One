using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelStages : MonoBehaviour
{
    public Color currentLevelColor;
    public Color completedLevelColor;
    public TextMeshProUGUI[] levelTexts;
    public Image[] levelImages;
    private GameManager _gm;

    private void Start()
    {
        _gm = GameManager.instance;
        SetEveryFiveLevelStages();
        SetCompletedLevelColor();
        SetCurrentLevelColor();
    }

    private void SetEveryFiveLevelStages()
    {
        int curLevel = _gm.level;
        while (curLevel % 5 != 1)
            curLevel--;

        for (int i = 0; i < levelTexts.Length; i++)
            levelTexts[i].text = (curLevel++).ToString();
    }

    private void SetCurrentLevelColor()
    {
        if (_gm.level % 5 != 0)
            levelImages[_gm.level % 5 - 1].color = currentLevelColor;
    }

    private void SetCompletedLevelColor()
    {
        for (int i = 0; i < levelImages.Length; i++)
        {
            if(i == _gm.level % 5 - 1)
                break;
            levelImages[i].color = completedLevelColor;
        }
    }
}
