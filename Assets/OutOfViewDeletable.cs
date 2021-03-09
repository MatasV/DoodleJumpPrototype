using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfViewDeletable : MonoBehaviour
{
    [SerializeField]
    private SharedVector3 playerPosition;

    [SerializeField] private Camera cameraToCheck;

    private Pool pool;
    public enum OutOfViewAction { Delete, PoolDisable}
    public OutOfViewAction action = OutOfViewAction.PoolDisable;
    public void Init(Pool poolRef, Camera cam)
    {
        pool = poolRef;
        cameraToCheck = cam;
    }

    private void OnEnable()
    {
        playerPosition.valueChangeEvent.AddListener(CheckPlayerPosition);
    }
    private void CheckPlayerPosition()
    {
        if (playerPosition.Value.y > transform.position.y && 
            cameraToCheck.ScreenToWorldPoint(Vector3.zero).y > transform.position.y)
        {
            switch (action)
            {
                case OutOfViewAction.Delete:
                    Delete();
                    break;
                case OutOfViewAction.PoolDisable:
                    Disable();
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void DeleteExternally()
    {
        switch (action)
        {
            case OutOfViewAction.Delete:
                Delete();
                break;
            case OutOfViewAction.PoolDisable:
                Disable();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    private void Delete()
    {
        Destroy(gameObject);
    }

    private void Disable()
    {
        foreach (Transform child in gameObject.transform)
            Destroy(child.gameObject);
        
        pool.DisableObj(gameObject.GetHashCode());
    }

    private void OnDisable()
    {
        playerPosition.valueChangeEvent.RemoveListener(CheckPlayerPosition);
    }
}
