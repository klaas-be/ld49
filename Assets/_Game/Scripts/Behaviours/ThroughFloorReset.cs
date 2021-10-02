using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThroughFloorReset : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Vector3 pos = other.transform.position;
        pos.y = 10;
        other.transform.position = pos;

        other.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }
}
