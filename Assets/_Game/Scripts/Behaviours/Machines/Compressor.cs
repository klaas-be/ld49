using _Game.Scripts.Behaviours;
using System.Collections;
using UnityEngine;

public class Compressor : Machine
{
    [SerializeField] Animator animator;
    [SerializeField] Transform Anchor;
    [SerializeField] Transform DropPoint;
    [SerializeField] float compressTime;

    ElementComponent currentElement;

    public override void Use(ElementComponent elementComponent)
    {
        canBeUsed = false;

        currentElement = elementComponent;
        currentElement.transform.SetParent(Anchor);
        currentElement.transform.localPosition = Vector3.zero;
        currentElement.OnPickUp();

        animator.SetTrigger("PressTrigger");
        StartCoroutine(CompressProcess());
    }

    private IEnumerator CompressProcess()
    {
        yield return new WaitForSeconds(compressTime);
        currentElement.transform.localScale = new Vector3(1, 0.4f, 1);
        ProcessEnd();
    }

    public void ProcessEnd()
    {
        currentElement._element.Compress();
        currentElement.transform.SetParent(null);
        currentElement.transform.position = DropPoint.position;

        currentElement.OnDrop();
        currentElement.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.value, Random.Range(-1, 1)), ForceMode.Impulse);
        currentElement.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), ForceMode.Impulse);
        currentElement = null;

        canBeUsed = true;
    }

    public override void Interact()
    {
        return;
    }
}
