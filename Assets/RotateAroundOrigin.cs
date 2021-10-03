using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundOrigin : MonoBehaviour
{


    public float speed = 5f;
    public Vector3 angle;
    public Vector3 target;
    // Update is called once per frame
    void Update()
    {
        gameObject.transform.RotateAround(target, angle, Time.deltaTime * speed);
    }
}