using System;
using System.Collections;
using UnityEngine;

public class NormalLegs : MonoBehaviour, IPowerUp
{
    public Vector3 jumpingPosition;
    public Vector3 kneelingPosition;
    
    public SpriteRenderer spriteRenderer;
    protected PlayerController player;

    public float jumpHeight;

    protected virtual void OnSideChanged(PlayerController.Side side)
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
    protected virtual void Jump(float jumpMult)
    {
        player.rb.velocity = Vector2.zero;
        player.rb.AddForce(Vector2.up * jumpHeight * jumpMult, ForceMode2D.Impulse);
        StartCoroutine(nameof(Kneel));
    }
    
    public virtual IEnumerator Kneel()
    {
        transform.localPosition = kneelingPosition;
        yield return new WaitForSeconds(0.05f);
        transform.localPosition = jumpingPosition;
    }

    private void OnDestroy()
    {
        if (player == null) return; 
        player.onJump -= Jump;
        player.onSideChanged -= OnSideChanged;
    }

    public virtual void OnPickUp(PlayerController p)
    {
        Debug.Log("PickedUp");
        player = p;
        player.EquipBootSlot(transform);
        transform.localPosition = jumpingPosition;
        transform.localScale = Vector3.one;
        player.onJump += Jump;
        player.onSideChanged += OnSideChanged;
        OnSideChanged(p.currentSide);

        var coll = GetComponent<Collider2D>();
        if (coll != null)
        {
            coll.enabled = false;
        }
    }
}