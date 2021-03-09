using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pool : MonoBehaviour
{
    private readonly Dictionary<int, GameObject> poolDictionary = new Dictionary<int, GameObject>();
    public void Init(int size, GameObject obj)
    {
        for (int i = 0; i < size; ++i)
        {
            var clone = Instantiate(obj);
            poolDictionary.Add(clone.GetHashCode(), clone);
            clone.SetActive(false);
        }
    }

    public GameObject GetObject(GameObject objToSpawn)
    {
        foreach (var obj in poolDictionary)
        {
            if (!obj.Value.activeInHierarchy)
            {
                obj.Value.SetActive(true);
                return obj.Value;
            }
        }
        
        var newObj = Instantiate(objToSpawn);
        poolDictionary.Add(newObj.GetHashCode(), newObj);
        newObj.SetActive(true);
        
        return newObj;
    }

    public bool DisableObj(int objHash)
    {
        if (poolDictionary.TryGetValue(objHash, out var objToDestroy))
        {
            objToDestroy.SetActive(false);
            return true;
        }
        Debug.LogWarning("Couldn't find required hash to disable, wtf?");
        return false;
        
    }
}