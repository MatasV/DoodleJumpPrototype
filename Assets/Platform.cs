using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, IPlatform
{
    [SerializeField] protected ThemeDatabase themeDatabase;
    
    public void Jumped()
    {
        
    }

    protected virtual void OnEnable()
    {
        GetComponent<SpriteRenderer>().sprite = themeDatabase.CurrentTheme.regularPlatform;
    }
}