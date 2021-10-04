using _Game.Scripts.Behaviours;
using _Game.Scripts.Classes;
using NaughtyAttributes;
using System.Collections;
using TMPro;
using UnityEngine;

public class Combiner : Machine
{
    [SerializeField] Animator animator;
    [SerializeField] Transform AnchorA, AnchorB, AnchorC, AnchorOut;

    [ReadOnly]
    [SerializeField] ElementComponent elementA, elementB, elementC;
    [SerializeField] float combineTime;
    [SerializeField] bool isPoof = false;

    [SerializeField] private AudioSource _audioSource; 

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
            animator.SetBool("IsPoof", false); isPoof = false;
             canBeUsed = false;
            animator.SetTrigger("CombinerTrigger");
            Element e = CombineRecipes.GetResultOf(elementA._element, elementB._element);
            if (e == null)
            {
                animator.SetBool("IsPoof", true); isPoof = true;
            }

            StartCoroutine(CombineProcess());
        }
    }

    private IEnumerator CombineProcess()
    {
        _audioSource.Play();
        yield return new WaitForSeconds(combineTime*0.8f);
        ProcessMiddle();
        _audioSource.Stop();
    }

    public void ProcessMiddle()
    {
        Element e = CombineRecipes.GetResultOf(elementA._element, elementB._element);

        if (!isPoof)
        {
            Destroy(elementA.gameObject);
            Destroy(elementB.gameObject);
        }
        else
        {
            elementA.transform.SetParent(null);
            elementA.OnDrop();
            elementA.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.value, Random.Range(-1, 1)), ForceMode.Impulse);
            elementA.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), ForceMode.Impulse);

            elementB.transform.SetParent(null);
            elementB.OnDrop();
            elementB.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(-1, 1), Random.value, Random.Range(-1, 1)), ForceMode.Impulse);
            elementB.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)), ForceMode.Impulse);
        }

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

    public override void Interact()
    {
        return;
    }
}

