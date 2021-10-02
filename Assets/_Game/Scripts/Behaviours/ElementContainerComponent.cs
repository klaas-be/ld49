using System.Collections.Generic;
using _Game.Scripts.Classes;
using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    public class ElementContainerComponent : MonoBehaviour
    {
        public int maxStack = 5;
        public int maxWalkStack = 2;

        private ElementContainer _elementContainer;
        public List<ElementSlot> ElementSlots;

        private void Awake()
        {
            _elementContainer = new ElementContainer(maxStack, maxWalkStack); 
        }

        public bool Add(ElementComponent element)
        {
            if (_elementContainer.Add(element))
            {
                var slotIndex = _elementContainer.carryingElements.Count - 1;
                element.transform.position = ElementSlots[slotIndex].transform.position;
                element.transform.parent = ElementSlots[slotIndex].transform;
                element.GetComponent<Rigidbody>().isKinematic = true;
                element.GetComponent<Collider>().enabled = false;
                return true;
               
            }

            return false;
        }
        

        //use element with something
        public void Use(Element e)
        {
            //_elementContainer.
        }
        
        /// <summary>
        /// Drop Elements to the ground
        /// </summary>
        public void Drop()
        {
            
        }
        
    }
}