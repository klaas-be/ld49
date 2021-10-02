using System.Collections.Generic;
using _Game.Scripts.Classes;
using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    public class ElementContainerComponent : MonoBehaviour
    {
        [NaughtyAttributes.ReadOnly]
        public List<Element> carryingElements = new List<Element>();
        public int maxStack = 5;
        public int maxWalkStack = 2; 
        
        public void Add(Element element)
        {
            if (carryingElements.Count < maxStack)
            {
                carryingElements.Add(element);
            }
        }


        /// <summary>
        /// Use an Element with another object
        /// </summary>
        public void Use()
        {
            
        }
        
        /// <summary>
        /// Drop Elements to the ground
        /// </summary>
        public void Drop()
        {
            
        }
        
    }
}