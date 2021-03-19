using UnityEngine;

public class RegularPlatform : Platform, ICanSpawnPowerUp
{
    private Vector3 spawnablePosition;
    public Vector3 GetSpawnablePosition()
    {
        return spawnablePosition;
    }
}