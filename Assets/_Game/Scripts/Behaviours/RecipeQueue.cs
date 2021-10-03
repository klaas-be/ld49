using System.Collections.Generic;
using _Game.Scripts.Classes;
using _Game.Scripts.ScriptableObjects;
using NaughtyAttributes;
using UnityEngine;

namespace _Game.Scripts.Behaviours
{
    public class RecipeQueue : MonoBehaviour
    {
        public List<Recipe> recipePool;
        public List<Recipe> queue;

        [Button]
        public void PickRecipeToQueue()
        {
            if(recipePool.Count == 0) return;
            var picked = recipePool[Random.Range(0, recipePool.Count)];
            queue.Add(picked);
            foreach (var e in picked.toSpawn)
            {
                var element = ElementSpawner.Instance.Spawn(e.elementType, transform.position); 
                GetComponent<Crater>().ThrowNewElement(element);
            }
        }

        public void CheckIfRequested(Element element)
        {
            var completed = queue.Find(recipe => recipe.requested.elementType == element.elementType);
            if(completed != null)
                queue.Remove(completed); 
        }
    }
}