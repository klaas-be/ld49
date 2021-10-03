using _Game.Scripts.Behaviours;
using NaughtyAttributes;
using UnityEngine;

public class UseAbility : MonoBehaviour
{
    [SerializeField] private ElementContainerComponent _elementContainerComponent;
    [ReadOnly] [SerializeField] private Machine currentMachine;

    bool useFlag = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (currentMachine && currentMachine.canBeUsed)
            {
                useFlag = true;
            }
        }
    }

    private void FixedUpdate()
    {
        if (useFlag)
        {
            useFlag = false;
            _elementContainerComponent.InteractWith(currentMachine);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Machine>(out var machine))
        {
            currentMachine = machine;            
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Machine>(out var machine))
        {
            currentMachine = machine;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Machine>(out var machine))
        {
            currentMachine = null;
        }
    }
}
