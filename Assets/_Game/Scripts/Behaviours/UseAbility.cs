using _Game.Scripts.Behaviours;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UseAbility : MonoBehaviour
{
    [SerializeField] private ElementContainerComponent _elementContainerComponent;
    [ReadOnly] [SerializeField] private Machine currentMachine;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMachine && currentMachine.canBeUsed)
            {
                Debug.Log("Use on " + currentMachine.name);
                _elementContainerComponent.Use(currentMachine);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Machine>(out var machine) & _elementContainerComponent.CarriesElement)
        {
            currentMachine = machine;            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Machine>(out var machine) & _elementContainerComponent.CarriesElement)
        {
            currentMachine = null;
        }
    }
}
