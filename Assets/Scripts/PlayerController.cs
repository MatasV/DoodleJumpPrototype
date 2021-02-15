using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public ThemeDatabase themeDatabase;
    private ThemeData currentTheme;
    public Rigidbody2D rb;
    public float sideWaysSpeed;

    public Collider2D mainCollider;
    private Camera mainCam;

    public SharedVector3 playerPosition;

    [Header("Renderers")]
    public SpriteRenderer mainPlayerRenderer;
    [Header("Powerup Slots")]
    public Transform headSlot;
    public Transform backSlot;
    public Transform bootSlot;
    
    public enum Side {Left, Right};
    public Side currentSide;
    
    public delegate void OnJump(float jumpMult);
    public OnJump onJump;

    public delegate void OnSideChanged(Side currentSide);
    public OnSideChanged onSideChanged;

    public GameObject legsGameObject;

    private void Awake()
    {
        playerPosition.Value = transform.position;
    }

    private void Start()
    {
        mainCam = Camera.main;
        mainCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        mainPlayerRenderer = GetComponent<SpriteRenderer>();
        currentTheme = themeDatabase.CurrentTheme;
        mainPlayerRenderer.sprite = currentTheme.doodle;
        SetupPowerups();
    }
    
    private void SetupPowerups()
    {
        EquipBootSlot(null);
        EquipBackSlot(null);
        EquipHeadSlot(null);
        Instantiate(legsGameObject).GetComponent<NormalLegs>().OnPickUp(this);
    }
    private void Update()
    {
        playerPosition.Value = transform.position;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.position += Vector2.left * sideWaysSpeed;
            mainPlayerRenderer.flipX = false;
            currentSide = Side.Left;
            onSideChanged.Invoke(currentSide);
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.position += Vector2.right * sideWaysSpeed;
            mainPlayerRenderer.flipX = true;
            currentSide = Side.Right;
            onSideChanged.Invoke(currentSide);
        }

        if (transform.position.x < mainCam.ScreenToWorldPoint(Vector2.zero).x)
        {
            Vector2 targetPos = mainCam.ScreenToWorldPoint(new Vector2(mainCam.pixelWidth, mainCam.WorldToScreenPoint(transform.position).y));
            transform.position = targetPos;
        } else if (transform.position.x > mainCam.ScreenToWorldPoint(new Vector2(mainCam.pixelWidth, 0)).x)
        {
            Vector2 targetPos = mainCam.ScreenToWorldPoint(new Vector2(0, mainCam.WorldToScreenPoint(transform.position).y));
            transform.position = targetPos;
        }
    }

    public void EquipBootSlot(Transform child)
    {
        foreach (Transform c in bootSlot)
        {
            Destroy(c.gameObject);
        }
        if(child != null)
            child.SetParent(bootSlot);
    }

    public void EquipBackSlot(Transform child)
    {
        foreach (Transform c in backSlot)
        {
            Destroy(c.gameObject);
        }
        if(child != null)
            child.SetParent(backSlot);
    }

    public void EquipHeadSlot(Transform child)
    {
        foreach (Transform c in backSlot)
        {
            Destroy(c.gameObject);
        }
        if(child != null)
            child.SetParent(headSlot);
    }
    
    private void LateUpdate()
    {
        var desiredPos = transform.position;
        desiredPos.x = 0f;
        desiredPos.z = -10f;
        mainCam.transform.position = desiredPos;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("Trigger");
        if (other.transform.CompareTag("PowerUp"))
        {
            var powerUp = other.transform.GetComponent<IPowerUp>();
            if (powerUp == null) return;
            powerUp.OnPickUp(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Platform"))
        {
            var jumpStrength = other.gameObject.GetComponent<IPlatform>().Jumped();
            if (onJump?.GetInvocationList().Length > 0 && jumpStrength > 0)
            {
                onJump.Invoke(jumpStrength);
            }
        }
    }
}
