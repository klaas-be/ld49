using _Game.Scripts.Behaviours;
using System.Collections;
using System.Collections.Generic;
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
        currentElement = null;

        canBeUsed = true;
    }
}
