using System.Collections.Generic;
using _Game.Scripts.Behaviours;
using _Game.Scripts.Classes;
using _Game.Scripts.Util;
using NaughtyAttributes;
using UnityEngine;

public class ElementSpawner : MonoSingleton<ElementSpawner>
{
    public GameObject elementPrefab;
    

    public List<ElementSettings> ElementSettings = new List<ElementSettings>();
    public List<StatusIcon> StatusIcons = new List<StatusIcon>(); 
    private static ElementSpawner _instance;


    [Button()]
    private void SpawnUranium()
    {
        Spawn(Element.ElementType.Uran, transform.position);
    }
    public ElementComponent Spawn(Element.ElementType type, Vector3 position)
    {
        GameObject instance =An.ElementGameObject.
            OfType(type).
            FromModel(ElementSettings.Find(settings => settings.Type == type).model);

        instance.transform.position = position;

        return instance.GetComponent<ElementComponent>();
    }
}

[System.Serializable]
public struct ElementSettings
{
    public Element.ElementType Type;
    public GameObject model;
    public Sprite icon; 

}

[System.Serializable]
public struct StatusIcon
{
    public Element.ElementStatus status;
    public Sprite icon; 
}
