using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThemeData : ScriptableObject
{
    [Serializable]
    public struct PlatformGameObject
    {
        public GameObject platform;
        public float spawnPercentage;
    }

    [Serializable]
    public struct PowerupGameObject
    {
        public GameObject powerUp;
        public float spawnPercentage;
    }

    public PowerupGameObject[] powerUps;
    public Sprite doodleJumpingSprite;
    public Sprite doodleShootSprite;
    public GameObject doodleBulletGameObject;
    public Sprite shooterSprite;
    public PlatformGameObject[] platformGameObjects;
    public Sprite doodleSnout;
    public Sprite regularPlatformSprite;
    public Sprite movingPlatform;
    public Sprite[] breakingPlatformSprite;
    public Sprite[] deathSprites;
    public Sprite doodle;
    public Sprite doodleLegsKneeling;
    public Sprite doodleLegsJumping;
    public Sprite[] springBootsSpriteAnimation;
    public GameObject legsGameObject;
    public Sprite[] propellerAnimation;
    public Sprite[] jetpackAnimation;
    
}
