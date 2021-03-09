using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Propeller : MonoBehaviour, IPowerUp, ISpawnable
{
    public SpriteRenderer spriteRenderer;
    private PlayerController player;
    public Vector3 requiredPosition;
    public float powerUpTime;
    public ThemeDatabase themeDatabase;
    public float flySpeed;
    [SerializeField] private Vector3 spawnablePosition;
    public void OnPickUp(PlayerController p)
    {
        if (!p.CanEquipHead) return;
        
        player = p;
        player.EquipHeadSlot(transform);
        transform.localPosition = requiredPosition;
        transform.localScale = Vector3.one;
        player.onSideChanged += OnSideChanged;
        OnSideChanged(p.currentSide);
        
        var coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }

        StartCoroutine(nameof(PropellerFlight));
    }

    private void OnDestroy()
    {
        if (player?.onSideChanged != null) player.onSideChanged -= OnSideChanged;
    }

    private IEnumerator PropellerFlight()
    {
        player.DisableCollisions();
        player.TurnOffGravity();
        var timer = 0f;
        var animationIndex = 0;
        while (timer < powerUpTime)
        {
            player.transform.position += Vector3.up * flySpeed;
            timer+=Time.deltaTime;
            spriteRenderer.sprite = themeDatabase.CurrentTheme.propellerAnimation[animationIndex];
            animationIndex++;
            if(animationIndex > themeDatabase.CurrentTheme.propellerAnimation.Length - 1) animationIndex = 0;
            yield return null;
        }
        player.EnableCollisions();
        player.TurnOnGravity();
        Destroy(gameObject);
    }
    protected void OnSideChanged(PlayerController.Side side)
    {
        switch (side)
        {
            case PlayerController.Side.Left:
                spriteRenderer.flipX = false;
                break;
            case PlayerController.Side.Right:
                spriteRenderer.flipX = true;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }
    public virtual Vector3 GetSpawnablePosition()
    {
        return spawnablePosition;
    }
}
