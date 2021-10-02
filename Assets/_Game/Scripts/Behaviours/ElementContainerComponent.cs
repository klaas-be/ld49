using System.Collections.Generic;
using _Game.Scripts.Classes;
using NaughtyAttributes;
using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    public class ElementContainerComponent : MonoBehaviour
    {
        public int maxStack = 5;
        public int maxWalkStack = 2;
        
        public List<ElementSlot> ElementSlots = new List<ElementSlot>();
        public List<ElementComponent> carryingElements = new List<ElementComponent>();

        
        public bool Add(ElementComponent element)
        {
            
            if (carryingElements.Count < maxStack)
            {
                carryingElements.Add(element);
                Debug.Log("Carrying Elements: "+ carryingElements.Count);
                PickupElement(element);
                return true;
            }

            return false;
        }

        
        public ElementComponent RemoveLastAdded()
        {
            var e = carryingElements[carryingElements.Count-1]; 
            carryingElements.RemoveAt(carryingElements.Count-1);
            return e;
        }
        
        
        private void PickupElement(ElementComponent element)
        {
            var slotIndex = carryingElements.Count - 1;
            element.transform.position = ElementSlots[slotIndex].transform.position;
            element.transform.parent = ElementSlots[slotIndex].transform;
            element.OmPickUp();
        }


        //use element with something
        public void Use(Element e)
        {
            //_elementContainer.
        }
        
        
        [Button("Drop Last")]
        /// <summary>
        /// Drop Elements to the ground
        /// </summary>
        public void Drop()
        {
            var slotIndex = carryingElements.Count-1;
            
            carryingElements[slotIndex].transform.parent = null;
         carryingElements[slotIndex].OnDrop();
         
         var element =  RemoveLastAdded();
            
           
            
        }
        
    }
}