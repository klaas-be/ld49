using System.Collections;
using System.Collections.Generic;
using _Game.Scripts.Behaviours;
using Unity.VisualScripting;
using UnityEngine;

namespace _Game.Scripts.Classes
{
    [System.Serializable]
    public class ElementContainer
    {
        public List<Element> carryingElements = new List<Element>();

        public int maxStack;
        public int maxWalkStack; 
        public ElementContainer(int maxStack, int maxWalkStack)
        {
            this.maxStack = maxStack;
            this.maxWalkStack = maxWalkStack; 
        }


        public bool Add(ElementComponent element)
        {
            if (carryingElements.Count < maxStack)
            {
                carryingElements.Add(element._element);
                Debug.Log("Carrying Elements: "+ carryingElements.Count);
                return true;
            }

            return false; 
        }

        public Element RemoveLastAdded()
        {
            var e = carryingElements[carryingElements.Count]; 
            carryingElements.RemoveAt(carryingElements.Count-1);
            return e;
        }
    }
}