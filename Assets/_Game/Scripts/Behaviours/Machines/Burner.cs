using _Game.Scripts.Behaviours;
using System.Collections;
using UnityEngine;

public class Burner : Machine
{
    [SerializeField] Animator animator;
    [SerializeField] Transform Anchor;
    [SerializeField] Transform DropPoint;
    [SerializeField] float burnTime;

    ElementComponent currentElement;

    public override void Use(ElementComponent elementComponent)
    {
        canBeUsed = false;

        currentElement = elementComponent;
        currentElement.transform.SetParent(Anchor);
        currentElement.transform.localPosition = Vector3.zero;
        currentElement.OnPickUp();

        animator.SetTrigger("BurnerTrigger");
        StartCoroutine(BurnProcess());
    }

    private IEnumerator BurnProcess()
    {
        yield return new WaitForSeconds(burnTime);
        ProcessEnd();
    }

    public void ProcessEnd()
    {
        currentElement._element.Burn();
        currentElement.transform.SetParent(null);
        currentElement.transform.position = DropPoint.position;

        currentElement.OnDrop();
        currentElement.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.value, Random.Range(-1, 1)), ForceMode.Impulse);
        currentElement.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), ForceMode.Impulse);
        currentElement = null;

        canBeUsed = true;
    }
}
