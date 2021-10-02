using _Game.Scripts.Behaviours;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static _Game.Scripts.Classes.Element;

public class Compressor : Machine
{
    [SerializeField] Transform Anchor;
    [SerializeField] Transform DropPoint;
    [SerializeField] float compressTime;

    ElementComponent currentElement;

    public override void Use(ElementComponent elementComponent)
    {
        currentElement = elementComponent;
        currentElement.transform.SetParent(Anchor);
        currentElement.OnPickUp();

        StartCoroutine(CompressProcess());
    }

    private IEnumerator CompressProcess()
    {
        yield return new WaitForSeconds(compressTime);
        ProcessEnd();
    }

    public void ProcessEnd()
    {
        currentElement._element.Compress();
        currentElement.transform.SetParent(DropPoint);
        currentElement.OnDrop();
        currentElement = null;
    }
}
