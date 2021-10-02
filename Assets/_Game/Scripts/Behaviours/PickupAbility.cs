using System;
using _Game.Scripts.Classes;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    public class PickupAbility : MonoBehaviour
    {
        [SerializeField] private ElementContainerComponent _eLementContainerComponent;
        
        private void PickUp(Element element)
        {
            _eLementContainerComponent.Add(element); 
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ( other.TryGetComponent<ElementComponent>(out var c))
            {
                PickUp(c._element);
            }
        }
    }
    
}