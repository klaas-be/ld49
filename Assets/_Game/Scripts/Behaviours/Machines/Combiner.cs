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
            elementA.OnPickUp();
        }
        else if (!elementB)
        {
            elementB = elementComponent;
            elementB.transform.SetParent(AnchorB);
            elementB.transform.localPosition = Vector3.zero;
            elementB.OnPickUp();
        }

        if (elementA && elementB)
        {
            canBeUsed = false;
            animator.SetTrigger("CombinerTrigger");
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
        elementC = ElementSpawner.Instance.Spawn(Recipes.GetResultOf(elementA._element, elementB._element).elementType, AnchorC.position);
        elementC.transform.SetParent(AnchorC);
        elementC.transform.localPosition = Vector3.zero;
        elementC.OnPickUp();

        Destroy(elementA.gameObject);
        Destroy(elementB.gameObject);

        elementA = null;
        elementB = null;

        StartCoroutine(ProcessEnd());
    }

    private IEnumerator ProcessEnd()
    {
        yield return new WaitForSeconds(combineTime *0.2f);

        elementC.transform.SetParent(null);
        elementC.transform.position = AnchorOut.position;

        elementC.OnDrop();
        elementC = null;

        canBeUsed = true;
    }
}

