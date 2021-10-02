using _Game.Scripts.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shocker : Machine
{
    [SerializeField] Animator animator;
    [SerializeField] Transform Anchor;
    [SerializeField] Transform DropPoint;
    [SerializeField] float shockTime;

    ElementComponent currentElement;

    public override void Use(ElementComponent elementComponent)
    {
        canBeUsed = false;

        currentElement = elementComponent;
        currentElement.transform.SetParent(Anchor);
        currentElement.transform.localPosition = Vector3.zero;
        currentElement.OnPickUp();

        animator.SetTrigger("ShockerTrigger");
        StartCoroutine(ShockProcess());
    }

    private IEnumerator ShockProcess()
    {
        yield return new WaitForSeconds(shockTime);
        ProcessEnd();
    }

    public void ProcessEnd()
    {
        currentElement._element.Electrocute();
        currentElement.transform.SetParent(null);
        currentElement.transform.position = DropPoint.position;

        currentElement.OnDrop();
        currentElement = null;

        canBeUsed = true;
    }
}
