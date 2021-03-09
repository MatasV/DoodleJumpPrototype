using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemy : Enemy
{
    public float amplitude;
    public Side side;
    public enum Side{Horizontal,Vertical}

    private float coordOne;
    private float coordTwo;
    
    private void OnEnable()
    {
        switch(side)
        {
            case Side.Horizontal:
                coordOne = transform.position.x - amplitude / 2;
                coordTwo = transform.position.x + amplitude / 2;
                break;
            case Side.Vertical:
                coordOne = transform.position.y - amplitude / 2;
                coordTwo = transform.position.y + amplitude / 2;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void Update()
    {
        switch (side)
        {
            case Side.Horizontal:
                transform.position = new Vector3( coordOne + Mathf.PingPong(Time.unscaledTime, coordTwo * 2), transform.position.y, transform.position.z ); 
                break;
            case Side.Vertical:
                transform.position = new Vector3(transform.position.x, coordOne + Mathf.PingPong(Time.unscaledTime, coordTwo ), transform.position.z ); 
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}
