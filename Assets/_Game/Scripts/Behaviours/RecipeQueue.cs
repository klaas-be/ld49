using System.Collections.Generic;
using System.Linq;
using _Game.Scripts.Classes;
using _Game.Scripts.ScriptableObjects;
using NaughtyAttributes;
using Unity.VisualScripting;
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

        [Header("Queue Element display")]
        public Transform QueueDisplayParent;

        public GameObject ElementDisplayPrefab;

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
            
           foreach (Transform o in QueueDisplayParent.transform)
           {
               Destroy(o.gameObject);
           }

           foreach (var recipe in queue)
           {
               var elementDisplayGo = Instantiate(ElementDisplayPrefab, QueueDisplayParent, false);
               var elementDisplay = elementDisplayGo.GetComponent<ElementDisplay>();
               
               elementDisplay.element = recipe.requested;
               elementDisplay.Icon.sprite = ElementSpawner.Instance.ElementSettings
                   .Find(settings => settings.Type == recipe.requested.elementType).icon;
               elementDisplay.Init();
           }

        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(spawnPoint, 0.2f);
        }
    }
}