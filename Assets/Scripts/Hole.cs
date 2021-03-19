using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hole : Enemy
{
    private PlayerController player;
    
    [SerializeField] private float scaleSpeed;
    [SerializeField] private float moveSpeed;
    public override void OnTouched(MonoBehaviour whoTouched)
    {
        if (whoTouched is PlayerController player)
        {
            this.player = player;
            
            if(player.CanDie())
                StartCoroutine(nameof(ToTheVoid));
        }
    }

    private IEnumerator ToTheVoid()
    {
        player.ResetVelocity();
        player.TurnOffGravity();
        player.LockControls();
        var startingTime = Time.time;
        var journeyLength = Vector3.Distance(player.transform.position, transform.position);
        var startingPos = player.transform.position;
        
        var startingScale = player.transform.localScale;
        var scaleLength = Vector3.Distance(startingScale, Vector3.zero);
        
        while (true)
        {
            var distCovered = (Time.time - startingTime) * moveSpeed;
            var scaleCovered = (Time.time - startingTime) * scaleSpeed;
            
            var fractionOfJourney = distCovered / journeyLength;
            player.transform.position = Vector3.Lerp(startingPos, transform.position, fractionOfJourney);

            var fractionOfScale = scaleCovered / scaleLength;
            player.transform.localScale = Vector3.Lerp(startingScale, Vector3.zero, fractionOfScale);

            yield return null;
            if (Vector3.Distance(player.transform.position,transform.position) < 0.1f) break;
        }
        player.GameOver();
    }
}