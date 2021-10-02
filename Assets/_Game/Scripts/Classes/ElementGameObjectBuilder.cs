﻿using _Game.Scripts.Behaviours;
using UnityEngine;

namespace _Game.Scripts.Classes
{
    public class ElementGameObjectBuilder
    {
        private Element.ElementType _type;
        private Element.ElementStatus _status;
        private GameObject prefab; 

        public ElementGameObjectBuilder OfType(Element.ElementType type)
        {
            this._type = type;
            return this; 
        }
        
        public ElementGameObjectBuilder WithStatus(Element.ElementStatus status)
        {
            _status = status;
            return this; 
        }

        public ElementGameObjectBuilder FromPrefab(GameObject prefab)
        {
            this.prefab = prefab;
            return this; 
        }

        public GameObject Build()
        {
            if (prefab == null)
            {
                GameObject go = new GameObject();
                var comp = go.AddComponent<ElementComponent>(); 
                comp._element=  new Element(this._type, this._status);
                return go; 
            }

            var instance = prefab;
            var elementComponent = instance.GetComponent<ElementComponent>();
            if(elementComponent == null)
                elementComponent = instance.AddComponent<ElementComponent>();
            elementComponent._element =  new Element(this._type, this._status);
            return instance;

        }

        public static implicit operator GameObject(ElementGameObjectBuilder builder)
        {
            return builder.Build();
        }
    }
}