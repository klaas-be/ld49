using System.Collections;
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

        public Transform spawnPoint;
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
                element.OnPickUp();
                element.transform.position = spawnPoint.position; 
                Rigidbody rigidbody = element.GetComponent<Rigidbody>();
                rigidbody.isKinematic = false;
                rigidbody.AddForce(forceVector.normalized * throwForce, ForceMode.Impulse);
                rigidbody.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1))*throwForce, ForceMode.Impulse);
                StartCoroutine(EnableCollider(element));
            }
            DisplayRequested();
        }
        private IEnumerator EnableCollider(ElementComponent elementComponent)
        {
            yield return new WaitForSeconds(0.5f);
            elementComponent.GetComponent<Collider>().enabled = true;
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
           requestElementDisplay.element = requestedElement;
           requestElementDisplay.Icon.sprite = iconToDisplay;
           requestElementDisplay.gameObject.SetActive(true);

        }
        
        private void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
            Gizmos.DrawLine(spawnPoint.position, spawnPoint.position + forceVector);
        }
    }
}