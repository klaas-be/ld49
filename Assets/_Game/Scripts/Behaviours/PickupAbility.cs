using _Game.Scripts.Classes;
using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    public class PickupAbility : MonoBehaviour
    {
        [SerializeField] private ElementContainerComponent _eLementContainerComponent;
        
        
        
        private bool PickUp(ElementComponent element)
        {
            return _eLementContainerComponent.Add(element); 
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if ( other.TryGetComponent<ElementComponent>(out var c))
            {
                if (!c.previouslyDropped)
                    PickUp(c);
            }
        }
    }
    
}