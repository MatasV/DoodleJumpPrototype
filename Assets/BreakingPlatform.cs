using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakingPlatform : Platform
{
    [SerializeField] private SpriteRenderer rend;
    
    public override float Jumped()
    {
        StartCoroutine(nameof(Break));
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        return 0f;
    }

    protected override void OnEnable()
    {
        gameObject.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = themeDatabase.CurrentTheme.breakingPlatformSprite[0];
    }

    private IEnumerator Break()
    {
        var index = 0;
        while (index < themeDatabase.CurrentTheme.breakingPlatformSprite.Length - 1)
        {
            index++;
            rend.sprite = themeDatabase.CurrentTheme.breakingPlatformSprite[index];
            yield return null;
        } 
        
    }
}