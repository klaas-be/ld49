using System.Collections;
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

        public Transform spawnPoint;
        public Transform targetPoint;
        public float throwForce; 

        [Header("Queue Element display")]
        public Transform QueueDisplayParent;

        public GameObject ElementDisplayPrefab;

        [Button]
        public void PickRecipeToQueue()
        {
            if(recipePool.Count == 0) return;
            var picked = recipePool[Random.Range(0, recipePool.Count)];
            queue.Add(picked);
            if (picked.toSpawn.Count > 0)
            {
                foreach (var e in picked.toSpawn)
                {
                    var element = ElementSpawner.Instance.Spawn(e.elementType, transform.position);
                    element.OnPickUp();
                    element.transform.position = spawnPoint.position;
                    Rigidbody rigidbody = element.GetComponent<Rigidbody>();
                    rigidbody.isKinematic = false;
                    rigidbody.AddForce((targetPoint.position - spawnPoint.position).normalized * throwForce, ForceMode.Impulse);
                    rigidbody.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * throwForce, ForceMode.Impulse);
                    StartCoroutine(EnableCollider(element));
                }
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
            {
                GameManager.instance.CraterAddBonusFromItem(completed.FinishInstabilityBonus);
                queue.Remove(completed);
            }
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
            Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
            Gizmos.DrawWireSphere(targetPoint.position, 0.2f);
            Gizmos.DrawLine(spawnPoint.position, targetPoint.position);
        }
    }
}
