using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 trajectory;
    [SerializeField] private float speed;
    public void Init(Vector3 requiredTrajectory)
    {
        trajectory = requiredTrajectory;
    }
    private void Update()
    {
        transform.position += trajectory * (Time.deltaTime * speed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("trigger entered");
        
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().OnTouched(this);
        }
    }
}
