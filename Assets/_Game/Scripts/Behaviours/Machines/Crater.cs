using _Game.Scripts.Behaviours;
using System.Collections;
using _Game.Scripts.Classes;
using _Game.Scripts.ScriptableObjects;
using UnityEngine;

public class Crater : Machine
{
    [SerializeField] Animator animator;
    [SerializeField] Transform DropPoint;
    [SerializeField] float throwForce;
    [SerializeField] float throwDelay;

    ElementComponent currentElement;

    public override void Use(ElementComponent elementComponent)
    {
        canBeUsed = false;
        currentElement = elementComponent;
        currentElement.GetComponent<Collider>().enabled = false;

        //Throw in Crater
        Rigidbody rigidbody = currentElement.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce((DropPoint.position - rigidbody.transform.position) * throwForce, ForceMode.Impulse);

        RemoveFromQueue(currentElement); 
        Destroy(currentElement.gameObject, 2f);
        currentElement = null;

        //animator.SetTrigger("CraterTrigger");
        StartCoroutine(ThrowProcess());

        
    }

    private void RemoveFromQueue(ElementComponent elementComponent)
    {
        RecipeQueue queue = GetComponent<RecipeQueue>();
        queue.CheckIfRequested(elementComponent._element); 
    }

    public void ThrowNewElement(ElementComponent element)
    {
        element.transform.position = DropPoint.position;

        element.OnDrop();
        element.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.value, Random.Range(-1, 1)), ForceMode.Impulse);
        element.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), ForceMode.Impulse);

        canBeUsed = true;
    }

    private IEnumerator ThrowProcess()
    {
        yield return new WaitForSeconds(throwDelay);
        canBeUsed = true;
    }
}
