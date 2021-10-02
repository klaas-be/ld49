using System;
using System.Collections;
using _Game.Scripts.Classes;
using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    [AddComponentMenu("Element")]
    public class ElementComponent : MonoBehaviour
    {
        public Element _element;

        public bool previouslyDropped; 
        public void OnPickUp()
        {
            GetComponent<Rigidbody>().isKinematic = true;
            GetComponent<Collider>().enabled = false;
        }


        public void OnDrop()
        {
            StartCoroutine(DelayedDrop()); 
        }

        public IEnumerator DelayedDrop()
        {
            GetComponent<Rigidbody>().isKinematic = false;
            GetComponent<Collider>().enabled = true;
            previouslyDropped = true; 
            yield return new WaitForSeconds(1);
            previouslyDropped = false;
        }        
    }
    
}