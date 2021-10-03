using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flickershock : MonoBehaviour
{
    public GameObject shock;
    // Update is called once per frame
    void Start() {
        InvokeRepeating("randomToggle", 1.0f, 0.5f);
    }

    private void randomToggle() {
        if (Random.value >= 0.5) {
            shock.SetActive(true);
        } 

        
        Invoke("turnOff", 0.1f);
    }

    private void turnOff() {
        shock.SetActive(false);
    }
}
