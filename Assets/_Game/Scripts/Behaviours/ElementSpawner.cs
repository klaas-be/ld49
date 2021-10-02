using System.Collections.Generic;
using _Game.Scripts.Behaviours;
using _Game.Scripts.Classes;
using NaughtyAttributes;
using UnityEngine;

public class ElementSpawner : MonoBehaviour
{
    public GameObject elementPrefab;

    public static ElementSpawner Instance;

    private void Awake() => Instance = this;

    public List<ElementSettings> ElementSettings = new List<ElementSettings>(); 


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

}
