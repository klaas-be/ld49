using _Game.Scripts.Behaviours;
using _Game.Scripts.Classes;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combiner : Machine
{
    [SerializeField] Animator animator;
    [SerializeField] Transform AnchorA, AnchorB, AnchorC, AnchorOut;

    [ReadOnly]
    [SerializeField] ElementComponent elementA, elementB, elementC;
    [SerializeField] float combineTime;

    public override void Use(ElementComponent elementComponent)
    {

        if (!elementA)
        {
            elementA = elementComponent;
            elementA.transform.SetParent(AnchorA);
            elementA.transform.localPosition = Vector3.zero;
            elementA.transform.localRotation = Quaternion.identity;
            elementA.OnPickUp();
        }
        else if (!elementB)
        {
            elementB = elementComponent;
            elementB.transform.SetParent(AnchorB);
            elementB.transform.localPosition = Vector3.zero;
            elementB.transform.localRotation = Quaternion.identity;
            elementB.OnPickUp();
        }

        if (elementA && elementB)
        {
            canBeUsed = false;
            animator.SetTrigger("CombinerTrigger");
            Element e = Recipes.GetResultOf(elementA._element, elementB._element);
            if (e == null)
            {
                animator.SetBool("IsPoof", true);
            }

            StartCoroutine(CombineProcess());
        }
    }

    private IEnumerator CombineProcess()
    {
        yield return new WaitForSeconds(combineTime*0.8f);
        ProcessMiddle();
    }

    public void ProcessMiddle()
    {
        Element e = Recipes.GetResultOf(elementA._element, elementB._element);

        Destroy(elementA.gameObject);
        Destroy(elementB.gameObject);

        elementA = null;
        elementB = null;

        if (e == null)
        {
            canBeUsed = true;
            return;
        }

        elementC = ElementSpawner.Instance.Spawn(e.elementType, AnchorC.position);
        elementC.transform.SetParent(AnchorC);
        elementC.transform.localPosition = Vector3.zero;
        elementC.OnPickUp();

        StartCoroutine(ProcessEnd());
    }

    private IEnumerator ProcessEnd()
    {
        yield return new WaitForSeconds(combineTime *0.2f);

        elementC.transform.SetParent(null);
        elementC.transform.position = AnchorOut.position;

        elementC.OnDrop();
        elementC.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1,1), Random.value, Random.Range(-1, 1)), ForceMode.Impulse);
        elementC.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), ForceMode.Impulse);
        elementC = null;

        canBeUsed = true;
    }
}

