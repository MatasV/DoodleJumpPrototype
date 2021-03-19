using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreDisplay : MonoBehaviour
{
    [SerializeField] private string preFix;
    [SerializeField] private TMP_Text scoreDisplay;
    [SerializeField] private SharedInt score;
    
    private void UpdateText()
    {
        scoreDisplay.text = preFix+score.Value.ToString();
    }

    private void Start()
    {
        score.valueChangeEvent.AddListener(UpdateText);
        UpdateText();
    }

    private void OnDestroy()
    {
        score.valueChangeEvent.RemoveListener(UpdateText);
    }
}
