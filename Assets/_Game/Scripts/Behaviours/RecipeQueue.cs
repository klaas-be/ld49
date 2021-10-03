using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Classes;
using _Game.Scripts.ScriptableObjects;
using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

namespace _Game.Scripts.Behaviours
{
    public class RecipeQueue : MonoBehaviour
    {
        public List<Recipe> recipePool;
        public List<Recipe> queue;

        public Vector3 spawnPoint;
        public float throwForce; 
        public Vector3 forceVector = new Vector3(0,1,-1);

        
        public ElementDisplay requestElementDisplay; 

        [Button]
        public void PickRecipeToQueue()
        {
            if(recipePool.Count == 0) return;
            var picked = recipePool[Random.Range(0, recipePool.Count)];
            queue.Add(picked);
            foreach (var e in picked.toSpawn)
            {
                var element = ElementSpawner.Instance.Spawn(e.elementType, transform.position);
                element.transform.position = spawnPoint; 
                Rigidbody rigidbody = element.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.AddForce(forceVector * throwForce, ForceMode.Impulse);
            }
            DisplayRequested();
        }
        

        public void CheckIfRequested(Element element)
        {
            var completed = queue.Find(recipe => recipe.requested.elementType == element.elementType);
            if(completed != null)
                queue.Remove(completed); 
            DisplayRequested();
        }

        [Button()]
        public void DisplayRequested()
        {
            if (queue.Count == 0)
            {
                requestElementDisplay.gameObject.SetActive(false);
                return;
            }
           var requestedElement =  queue.First().requested;
           var iconToDisplay =
               ElementSpawner.Instance.ElementSettings.Find(settings => settings.Type == requestedElement.elementType).icon; 
           Debug.Log(requestedElement);
           requestElementDisplay.element = requestedElement;
           requestElementDisplay.Icon.sprite = iconToDisplay;
           requestElementDisplay.gameObject.SetActive(true);

        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(spawnPoint, 0.2f);
        }
    }
}