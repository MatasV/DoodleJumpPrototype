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
    private Vector3 topLeft;
    private Vector3 topRight;
    [SerializeField] private ThemeDatabase themeDatabase;
    private ThemeData.PlatformGameObject[] platforms;
    private ThemeData.PowerupGameObject[] powerUps;
    private Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
    private Camera mainCamera;
    private void OnEnable()
    {
        mainCamera = Camera.main;
        topLeft = mainCamera.ScreenToWorldPoint(Vector3.zero);
        topRight = mainCamera.ScreenToWorldPoint(new Vector3(Camera.main.pixelWidth, 0, 0));
        playerPosition.valueChangeEvent.AddListener(CheckSpawningRequirements);
        startingY = 0;
        SetupPlatformSpawnChances();
        SetupPowerUpChances();
    }

    private void SetupPowerUpChances()
    {
        powerUps = new ThemeData.PowerupGameObject[themeDatabase.CurrentTheme.platformGameObjects.Length];
        
        Array.Copy(themeDatabase.CurrentTheme.powerUps, 0,  powerUps, 0, themeDatabase.CurrentTheme.powerUps.Length);
        
        if (powerUps.Length < 1) return;
        var percentage = powerUps[0].spawnPercentage;
        
        for (int i = 0; i < powerUps.Length; i++)
        {
            percentage += powerUps[i].spawnPercentage;
            powerUps[i].spawnPercentage = percentage;
        }
    }

    private void SetupPlatformSpawnChances()
    {
        platforms = new ThemeData.PlatformGameObject[themeDatabase.CurrentTheme.platformGameObjects.Length];
        
        Array.Copy(themeDatabase.CurrentTheme.platformGameObjects, 0,  platforms, 0, themeDatabase.CurrentTheme.platformGameObjects.Length);

        if (platforms.Length < 1) return;
        var percentage = platforms[0].spawnPercentage;

        var poolParent = new GameObject {name = "Pools"}.transform;


        
        for (int i = 0; i < platforms.Length; i++)
        {
            percentage += platforms[i].spawnPercentage;

            var pool = new GameObject().AddComponent<Pool>();
            pool.Init(20, platforms[i].platform);
            pool.name = $"{platforms[i].platform.name} Pool";
            pool.transform.SetParent(poolParent.transform);
            pools.Add(pool.name, pool);
            
            platforms[i].spawnPercentage = percentage;
        }
    }

    private void ChangeSpawnAmount()
    {
        spawningYFreq = Mathf.Clamp(playerPosition.Value.y / 25, 1, 2);
    }

    private void CheckSpawningRequirements()
    {
        ChangeSpawnAmount();

        if (playerPosition.Value.y < startingY + startingSpawningYOffset) return;
        if (playerPosition.Value.y > lastSpawnY + spawningYFreq)
        {
            SpawnTiles();
            lastSpawnY = playerPosition.Value.y;
        }
    }

    private void SpawnTiles()
    {
        var percentage = Random.Range(0,
            platforms[platforms.Length - 1].spawnPercentage);
        
        var powerupPercentage = Random.Range(0,
            powerUps[powerUps.Length - 1].spawnPercentage);

        
        var spawnX = Random.Range(topLeft.x, topRight.x);

        for (int j = 0; j < platforms.Length; j++)
        {
            if (percentage < platforms[j].spawnPercentage)
            {
                if (pools.TryGetValue($"{platforms[j].platform.name} Pool", out var poolToUse))
                {
                    var spawnedObject = poolToUse.GetObject(platforms[j].platform);
                    spawnedObject.transform.localPosition = new Vector3(spawnX, lastSpawnY + spawningYOffset, 0);
                    spawnedObject.transform.localRotation = Quaternion.Euler(Vector3.zero);
                    spawnedObject.GetComponent<OutOfViewDeletable>()?.Init(poolToUse, mainCamera);

                    foreach (var comp in spawnedObject.GetComponents(typeof(Component)))
                    {
                        if (comp.GetType().GetInterface(nameof(ICanSpawnPowerUp)) != null)
                        {
                            Debug.Log(comp.GetType() + "does implement interface");

                            for (int k = 0; k < powerUps.Length; k++)
                            {
                                if (powerupPercentage < powerUps[k].spawnPercentage)
                                {
                                    if (powerUps[k].powerUp != null)
                                    {
                                        SpawnPowerUpOnPlatform(powerUps[k].powerUp, spawnedObject);
                                        break;
                                    }
                                }
                            }
                        }
                    }

                    Debug.Log(spawnedObject.name + "does not implement interface");

                    break;
                }
                else
                {
                    Debug.LogWarning(
                        $"Could not spawn a requested spawnable by the name of {platforms[j].platform.name}");
                }

                return;
            }
        }

    }

    private void SpawnPowerUpOnPlatform(GameObject powerUp, GameObject spawnedObject)
    {
        var platformBounds = GetMaxBounds(spawnedObject);
        var leftX = platformBounds.min.x;
        var rightX = platformBounds.max.x;
        var top = platformBounds.center.y + platformBounds.extents.y;

        var powerUpBounds = GetMaxBounds(powerUp);
        
        var spawnPosition = new Vector3(Random.Range(leftX, rightX), top + powerUpBounds.extents.y, 0);

        Instantiate(powerUp, spawnPosition, Quaternion.identity, spawnedObject.transform);

    }

    private static Bounds GetMaxBounds(GameObject g) {
        var renderers = g.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return new Bounds(g.transform.position, Vector3.zero);
        var b = renderers[0].bounds;
        foreach (Renderer r in renderers) {
            b.Encapsulate(r.bounds);
        }
        return b;
    }
    
    private void OnDestroy()
    {
        playerPosition.valueChangeEvent.RemoveListener(CheckSpawningRequirements);
    }
}
