using _Game.Scripts.Behaviours;
using _Game.Scripts.Classes;
using NaughtyAttributes;
using UnityEngine;

public class ElementSpawner : MonoBehaviour
{
    public GameObject elementPrefab;

    public static ElementSpawner Instance;

    private void Awake() => Instance = this;

    
    [Button()]
    private void SawnElement()
    {
        Spawn(Element.ElementType.Uran, transform.position);
    }
    public void Spawn(Element.ElementType type, Vector3 position)
    {
        GameObject instance = (GameObject)Instantiate(An.ElementGameObject.FromPrefab(elementPrefab).OfType(type));

        instance.transform.position = position; 
    }
}
