using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighScoreDisplay : MonoBehaviour
{
    [SerializeField] private String preFix;
    [SerializeField] private TMP_Text highScoreDisplay;
    [SerializeField] private HighScore highScore;

    private void OnEnable()
    {
        highScoreDisplay.text = preFix+highScore.MaxScore.ToString();
    }
}
