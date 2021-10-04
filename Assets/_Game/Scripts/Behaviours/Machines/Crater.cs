using _Game.Scripts.Behaviours;
using System.Collections;
using _Game.Scripts.Classes;
using _Game.Scripts.ScriptableObjects;
using UnityEngine;
using NaughtyAttributes;

public class Crater : Machine
{
    [SerializeField] Animator animator;
    [SerializeField] Transform DropPoint;
    [SerializeField] Transform Lava;
    [SerializeField,ReadOnly] Vector3 LavaStartHeight;
    [SerializeField] float throwForce;
    [SerializeField] float throwDelay;

    ElementComponent currentElement;

    private void Start()
    {
        LavaStartHeight = Lava.position;
    }

    public override void Use(ElementComponent elementComponent)
    {
        canBeUsed = false;
        currentElement = elementComponent;
        currentElement.GetComponent<Collider>().enabled = false;

        //Throw in Crater
        Rigidbody rigidbody = elementComponent.GetComponent<Rigidbody>();
        rigidbody.isKinematic = false;
        rigidbody.AddForce((DropPoint.position - rigidbody.position).normalized * throwForce, ForceMode.Impulse);

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

    public override void Interact()
    {
        return;
    }

    public void SetLavaLevel(float to)
    {
        Lava.position = Vector3.Lerp(LavaStartHeight, LavaStartHeight + Vector3.up * 0.85f, Mathf.Clamp01(to));
    }
}
