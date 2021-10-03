using _Game.Scripts.Behaviours;
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static _Game.Scripts.Classes.Element;

public class ChestSpawner : Machine
{
    [SerializeField] ElementType spawnType;

    [SerializeField] Animator animator;
    [SerializeField] Transform ThrowTarget;
    [SerializeField] Transform SpawnPoint;
    [SerializeField] Collider UseCollider;
    [SerializeField] float throwForce;
    [SerializeField] float throwDelay;
    [SerializeField] float nextSpawnDelay;
    [Space(20)]
    [SerializeField] Transform HologramPoint;
    [SerializeField] Material holoMaterial;

    public void Start()
    {
        ElementComponent elementComponent = ElementSpawner.Instance.Spawn(spawnType, HologramPoint.position);
        elementComponent.OnPickUp();
        elementComponent.transform.SetParent(HologramPoint);
        elementComponent.GetComponent<MeshRenderer>().sharedMaterial = holoMaterial;
    }

    public override void Use(ElementComponent elementComponent)
    {
        return;
    }

    public override void Interact()
    {
        canBeUsed = false;
        animator.SetTrigger("LidTrigger");

        ElementComponent elementComponent = ElementSpawner.Instance.Spawn(spawnType, SpawnPoint.position);
        elementComponent.OnPickUp();

        StartCoroutine(SpawnThrowProcess(elementComponent));
    }

    private IEnumerator SpawnThrowProcess(ElementComponent elementComponent)
    {
        yield return new WaitForSeconds(throwDelay);
        elementComponent.GetComponent<Rigidbody>().isKinematic = false;

        Rigidbody rigidbody = elementComponent.GetComponent<Rigidbody>();
        rigidbody.AddForce((ThrowTarget.position - rigidbody.position).normalized * throwForce, ForceMode.Impulse);
        StartCoroutine(EnableCollider(elementComponent));

        yield return new WaitForSeconds(nextSpawnDelay);
        canBeUsed = true;
    }
    private IEnumerator EnableCollider(ElementComponent elementComponent)
    {
        yield return new WaitForSeconds(0.5f);
        elementComponent.GetComponent<Collider>().enabled = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(SpawnPoint.position, ThrowTarget.position);
        Gizmos.color = Color.white;
        Gizmos.DrawLine(transform.position, transform.position - transform.forward);
    }
}
