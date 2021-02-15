using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlatformGenerator : MonoBehaviour
{
    [SerializeField] private SharedVector3 playerPosition;
    [SerializeField] private float lastSpawnY;
    [SerializeField] private float startingSpawningYOffset;
    [SerializeField] private float spawningYOffset;
    [SerializeField] private float spawningYFreq;
    [SerializeField] private float startingY;
    [SerializeField] private int platformSpawnAmount;
    private Vector3 topLeft;
    private Vector3 topRight;
    [SerializeField] private ThemeDatabase themeDatabase;
    private ThemeData.PlatformGameObjects[] platforms;
    private void OnEnable()
    {
        topLeft = Camera.main.ScreenToWorldPoint(Vector3.zero);
        topRight = Camera.main.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, 0));
        playerPosition.valueChangeEvent.AddListener(CheckSpawningRequirements);
        startingY = 0;
        SetupPlatformSpawnChances();
    }

    private void SetupPlatformSpawnChances()
    {
        platforms = new ThemeData.PlatformGameObjects[themeDatabase.CurrentTheme.platformGameObjects.Length];
        
        Array.Copy(themeDatabase.CurrentTheme.platformGameObjects, 0,  platforms, 0, themeDatabase.CurrentTheme.platformGameObjects.Length);

        if (platforms.Length < 1) return;
        var percentage = platforms[0].spawnPercentage;

        for (int i = 0; i < platforms.Length; i++)
        {
            percentage += platforms[i].spawnPercentage;
            platforms[i].spawnPercentage = percentage;
        }
    }

    private void ChangeSpawnAmount()
    {
        platformSpawnAmount = Mathf.CeilToInt(Mathf.Clamp(1 / Mathf.Log(2, (playerPosition.Value.y / 3)), 1, 15));
    }

    private void CheckSpawningRequirements()
    {
        
        ChangeSpawnAmount();
        Debug.Log(playerPosition.Value.y);
        
        if (playerPosition.Value.y < startingY + startingSpawningYOffset) return;
        if (playerPosition.Value.y > lastSpawnY + spawningYFreq)
        {
            SpawnTiles();
            lastSpawnY = playerPosition.Value.y;
        }
    }

    private void SpawnTiles()
    {
        Debug.LogWarning("Spawning");
        var spawnX = Random.Range(topLeft.x, topRight.x);

        var percentage = Random.Range(0,
            platforms[platforms.Length - 1].spawnPercentage);
        
        for (int i = 0; i < platforms.Length; i++)
        {
            if (percentage < platforms[i].spawnPercentage) {
                Instantiate(platforms[i].platform, new Vector3(spawnX, lastSpawnY + spawningYOffset, 0), Quaternion.identity);
                return;
            }
        }

    }
    
    private void OnDestroy()
    {
        playerPosition.valueChangeEvent.RemoveListener(CheckSpawningRequirements);
    }
}
