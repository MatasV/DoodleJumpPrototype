using System;
using UnityEngine;
using UnityEngine.PlayerLoop;

class MovingPlatform : Platform, ICanSpawnPowerUp
{
   private Camera mainCam;
   public Vector3 leftSide;
   public Vector3 rightSide;
   private bool goingLeft;
   
   
   protected override void OnEnable()
   {
      GetComponent<SpriteRenderer>().sprite = themeDatabase.CurrentTheme.movingPlatform;
      mainCam = Camera.main;
      rightSide = mainCam.ScreenToWorldPoint(new Vector2(mainCam.pixelWidth, mainCam.WorldToScreenPoint(transform.position).y));
      leftSide = mainCam.ScreenToWorldPoint(new Vector2(0, mainCam.WorldToScreenPoint(transform.position).y));
   }
   
   private void Update()
   {
      transform.position = new Vector3( leftSide.x + Mathf.PingPong(Time.unscaledTime, rightSide.x * 2), transform.position.y, transform.position.z ); 
   }
   
}