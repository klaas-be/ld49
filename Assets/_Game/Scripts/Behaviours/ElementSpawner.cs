using _Game.Scripts.Behaviours;
using _Game.Scripts.Classes;
using UnityEngine;

public class ElementSpawner : MonoBehaviour
{
    private GameObject elementPrefab;

    public static ElementSpawner Instance;

    private void Awake() => Instance = this;

    public void Spawn(Element.ElementType type, Vector3 position)
    {
        var instance = Instantiate(elementPrefab);
        var elementComponent = instance.GetComponent<ElementComponent>();
        var element = new Element(type);

        elementComponent._element = element;

        instance.transform.position = position; 
    }
}
