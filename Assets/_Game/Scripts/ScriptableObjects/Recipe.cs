using System.Collections.Generic;
using _Game.Scripts.Classes;
using UnityEngine;

namespace _Game.Scripts.ScriptableObjects
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "ScriptableObjects/Recipe", order = 1)]
    public class Recipe : ScriptableObject
    {
        public List<Element> toSpawn; 
        //what the crater wants
        public Element requested; 
        
    }
}