using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public ThemeDatabase themeDatabase;
    private ThemeData currentTheme;
    public Rigidbody2D rb;
    public Collider2D col;
    public float sideWaysSpeed;

    public bool canMove = true;

    public Collider2D mainCollider;
    private Camera mainCam;

    public SharedVector3 playerPosition;
    public SharedInt score;

    [Header("Renderers")]
    public SpriteRenderer mainPlayerRenderer;
    [Header("Powerup Slots")]
    public Transform headSlot;
    public Transform backSlot;
    public Transform bootSlot;

    public enum Side { Left, Right };
    public Side currentSide;

    public delegate void OnJump(float jumpMult);
    public OnJump onJump;

    public delegate void OnSideChanged(Side currentSide);
    public OnSideChanged onSideChanged;

    public delegate void OnDead();
    public OnDead onDead;

    public GameObject legsGameObject;

    [SerializeField] private GameObject gameOverScreen;

    [Header("Shooting")]
    [SerializeField] private Transform shootingPosition;
    [SerializeField] private SpriteRenderer shooterSprite;

    [Header("Audio:")]
    [SerializeField] private float volume = 0.5f;
    [SerializeField] private AudioClip jumpSound;
    private AudioSource audioSource;

    
    public bool CanEquipHead => headSlot.childCount == 0;
    public bool CanEquipBack => backSlot.childCount == 0;
    public bool CanEquipLegs => bootSlot.childCount == 1 && bootSlot.GetChild(0).name == "Legs";

    private void Awake()
    {
        playerPosition.Value = transform.position;
        score.Value = 0;
    }
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        col = GetComponent<Collider2D>();
        canMove = true;
        mainCam = Camera.main;
        mainCollider = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        mainPlayerRenderer = GetComponent<SpriteRenderer>();
        currentTheme = themeDatabase.CurrentTheme;
        mainPlayerRenderer.sprite = currentTheme.doodle;
        shooterSprite.sprite = currentTheme.shooterSprite;
        shootingPosition.gameObject.SetActive(false);
        TurnOnGravity();
        SetupPowerups();
    }
    private void SetupPowerups()
    {
        EquipBootSlot(null);
        EquipBackSlot(null);
        EquipHeadSlot(null);
        Instantiate(legsGameObject).GetComponent<NormalLegs>().OnPickUp(this);
    }
    
    public void GameOver()
    {
        DisableCollisions();
        onDead?.Invoke();
        gameOverScreen.GetComponent<GameOverScreen>().ShowGameOverScreen();
        Debug.Log("Game Over");
    }
    public void LockControls()
    {
        canMove = false;
    }

    public void DisableCollisions()
    {
        col.enabled = false;
    }

    public void EnableCollisions()
    {
        col.enabled = true;
    }
    public void ResetVelocity()
    {
        rb.velocity = Vector3.zero;
    }
    public void TurnOnGravity()
    {
        rb.gravityScale = 1f;
    }
    public void TurnOffGravity()
    {
        rb.gravityScale = 0f;
    }
    private void Update()
    {
        var heightDelta = transform.position.y - playerPosition.Value.y;

        if (heightDelta > 0) score.Value += (int)(heightDelta * 100);
        playerPosition.Value = transform.position;
        
        if (!canMove) return;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.position += Vector3.left * sideWaysSpeed;
            mainPlayerRenderer.flipX = false;
            currentSide = Side.Left;
            onSideChanged.Invoke(currentSide);
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.position += Vector3.right * sideWaysSpeed;
            mainPlayerRenderer.flipX = true;
            currentSide = Side.Right;
            onSideChanged.Invoke(currentSide);
        }

        #if UNITY_ANDROID

        float xGyroInput = Input.acceleration.x;
        Vector3 tiltPos = new Vector3(xGyroInput, 0f, 0f);
        transform.position += tiltPos * sideWaysSpeed;

        #endif

         if (Input.GetMouseButtonDown(0))
         {
             var mousePos = mainCam.ScreenToWorldPoint(Input.mousePosition);
             StartCoroutine(Shoot(mousePos));
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

         if (transform.position.y < mainCam.ScreenToWorldPoint(Vector2.zero).y)
         {
             LockControls();
             GameOver();
         }
    }

    public bool CanDie()
    {
        return (headSlot.childCount == 0 && backSlot.childCount == 0);
    }
    private IEnumerator Shoot(Vector3 mousePos)
    {
        mousePos.z = 0;
        
        var relativePos = mousePos - shootingPosition.position;
        
        shootingPosition.gameObject.SetActive(true);
        mainPlayerRenderer.sprite = currentTheme.doodleShootSprite;
        var rot = Quaternion.LookRotation(relativePos, Vector3.back);

        rot.x = 0;
        rot.y = 0;

        shootingPosition.rotation = rot;

        Instantiate(currentTheme.doodleBulletGameObject, shootingPosition.position, Quaternion.identity).GetComponent<Bullet>().Init(relativePos.normalized);

        yield return new WaitForSeconds(0.3f);
        
        mainPlayerRenderer.sprite = currentTheme.doodle;
        shootingPosition.gameObject.SetActive(false);
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
        if(child != null)
            child.SetParent(backSlot);
    }

    public void EquipHeadSlot(Transform child)
    {
        
        if(child != null)
            child.SetParent(headSlot);
    }
    
    private void LateUpdate()
    {
        var desiredPos = transform.position;
        if (desiredPos.y < mainCam.transform.position.y)
        {
            desiredPos.y = mainCam.transform.position.y;
        }

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
        else if(other.transform.CompareTag("Enemy"))
        {
            var enemy = other.transform.GetComponent<Enemy>();
            if(enemy == null) return;
            enemy.OnTouched(this);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.transform.CompareTag("Platform"))
        {
            audioSource.PlayOneShot(jumpSound, volume);
            var jumpStrength = other.gameObject.GetComponent<IPlatform>().Jumped();
            if (onJump?.GetInvocationList().Length > 0 && jumpStrength > 0)
            {
                onJump.Invoke(jumpStrength);
            }
        }
    }
}
