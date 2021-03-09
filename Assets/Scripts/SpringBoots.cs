using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringBoots : NormalLegs, ISpawnable
{
    public SpriteRenderer leftBootRenderer;
    public SpriteRenderer rightBootRenderer;
    
    [SerializeField] private Vector3 spawnablePosition;
    [SerializeField] public ThemeDatabase themeDatabase;
    public float powerUpTime;
    private bool animDone;
    protected override void Jump(float jumpMult)
    {
        animDone = false;
        player.rb.velocity = Vector2.zero;
        player.rb.AddForce(Vector2.up * jumpHeight * jumpMult, ForceMode2D.Impulse);
        StartCoroutine(nameof(Kneel));
    }

    protected override void OnSideChanged(PlayerController.Side side)
    {
        switch (side)
        {
            case PlayerController.Side.Left:
                leftBootRenderer.flipX = false;
                rightBootRenderer.flipX = false;
                break;
            case PlayerController.Side.Right:
                leftBootRenderer.flipX = true;
                rightBootRenderer.flipX = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    public override IEnumerator Kneel()
    {
        var index = 0;
        var maxIndex = themeDatabase.CurrentTheme.springBootsSpriteAnimation.Length - 1;
        var goingBackwards = false;
        while (!animDone)
        {
            leftBootRenderer.sprite = themeDatabase.CurrentTheme.springBootsSpriteAnimation[index];
            rightBootRenderer.sprite = themeDatabase.CurrentTheme.springBootsSpriteAnimation[index];
            if (index < maxIndex && !goingBackwards)
            {
                index++;
            }
            else
            {
                goingBackwards = true;
                index--;
                if(index == 0) animDone = true;
            }

            yield return null;
        }
    }

    private void Reset()
    {
        Instantiate(themeDatabase.CurrentTheme.legsGameObject).GetComponent<NormalLegs>().OnPickUp(player);
    }
    private void OnDestroy()
    {
        if (player?.onSideChanged != null) player.onSideChanged -= OnSideChanged;
    }
    public override void OnPickUp(PlayerController p)
    {
        base.OnPickUp(p);
        Invoke(nameof(Reset), powerUpTime);
    }
    public Vector3 GetSpawnablePosition()
    {
        return spawnablePosition;
    }
}