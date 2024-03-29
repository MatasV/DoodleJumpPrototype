﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Jetpack : MonoBehaviour, IPowerUp, ISpawnable
{
    public Vector3 leftSidePosition;
    public Vector3 rightSidePosition;
    public SpriteRenderer spriteRenderer;
    private PlayerController player;
    public float powerUpTime;
    public ThemeDatabase themeDatabase;
    public float flySpeed;
    [SerializeField] private Vector3 spawnablePosition;
    public void OnPickUp(PlayerController p)
    {
        if (!p.CanEquipBack) return;
        
        player = p;
        player.EquipBackSlot(transform);
        OnSideChanged(player.currentSide);
        transform.localScale = Vector3.one;
        player.onSideChanged += OnSideChanged;
        OnSideChanged(p.currentSide);
        
        var coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }

        StartCoroutine(nameof(JetpackFlight));
    }

    private void OnSideChanged(PlayerController.Side side)
    {
        switch (side)
        {
            case PlayerController.Side.Left:
                spriteRenderer.flipX = false;
                transform.localPosition = rightSidePosition;
                break;
            case PlayerController.Side.Right:
                spriteRenderer.flipX = true;
                transform.localPosition = leftSidePosition;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(side), side, null);
        }
    }

    private void OnDestroy()
    {
        if (player?.onSideChanged != null) player.onSideChanged -= OnSideChanged;
    }
    public IEnumerator JetpackFlight()
    {
        player.DisableCollisions();
        player.TurnOffGravity();
        var timer = 0f;
        var animationIndex = 0;
        while (timer < powerUpTime)
        {
            player.transform.position += Vector3.up * flySpeed;
            timer+=Time.deltaTime;
            spriteRenderer.sprite = themeDatabase.CurrentTheme.jetpackAnimation[animationIndex];
            animationIndex++;
            if(animationIndex > themeDatabase.CurrentTheme.propellerAnimation.Length - 1) animationIndex = 0;
            yield return null;
        }
        player.EnableCollisions();
        player.TurnOnGravity();
        Destroy(gameObject);
    }
    public Vector3 GetSpawnablePosition()
    {
        return spawnablePosition;
    }
}