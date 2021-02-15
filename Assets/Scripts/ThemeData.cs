using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ThemeData : ScriptableObject
{
    [Serializable]
    public struct PlatformGameObjects
    {
        public GameObject platform;
        public float spawnPercentage;
    }

    public PlatformGameObjects[] platformGameObjects;
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
