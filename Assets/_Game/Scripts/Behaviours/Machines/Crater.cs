using _Game.Scripts.Behaviours;
using System.Collections;
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

        Destroy(currentElement.gameObject, 2f);
        currentElement = null;

        //animator.SetTrigger("CraterTrigger");
        StartCoroutine(ThrowProcess());
    }

    private IEnumerator ThrowProcess()
    {
        yield return new WaitForSeconds(throwDelay);
        canBeUsed = true;
    }
}
