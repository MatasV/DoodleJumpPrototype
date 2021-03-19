using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platform : MonoBehaviour, IPlatform
{
    [SerializeField] protected ThemeDatabase themeDatabase;
    public virtual float Jumped()
    {
        return 1f;
    }

    protected virtual void OnEnable()
    {
        GetComponent<SpriteRenderer>().sprite = themeDatabase.CurrentTheme.regularPlatformSprite;
    }
}