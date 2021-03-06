using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    public class PickupAbility : MonoBehaviour
    {
        [SerializeField] private ElementContainerComponent _elementContainerComponent;       
        
        
        private bool PickUp(ElementComponent element)
        {
            return _elementContainerComponent.Add(element); 
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ElementComponent>(out var c))
            {
                if (!c.previouslyDropped)
                    PickUp(c);
            }
        }
    }
    
}