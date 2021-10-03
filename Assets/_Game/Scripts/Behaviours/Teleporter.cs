using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleporter : MonoBehaviour
{
    [SerializeField] private Transform targetPos;
    [SerializeField] private LineRenderer lineRenderer;
    [ReadOnly] [SerializeField] private Transform player;
    [SerializeField] private bool teleport = false;
    [SerializeField] private float lineUp = 1.5f;

    private void LateUpdate()
    {
        if (teleport)
        {
            teleport = false;
            CharacterController charController = player.gameObject.GetComponent<CharacterController>();
            charController.enabled = false;
            player.position = targetPos.position + Vector3.up;
            charController.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            player = other.gameObject.transform;
            teleport = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position, targetPos.position);
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(targetPos.position, 0.1f);
        lineRenderer.SetPosition(1, Vector3.Lerp(lineRenderer.GetPosition(0), lineRenderer.GetPosition(2), 0.5f) + Vector3.up * lineUp);
        lineRenderer.SetPosition(2, targetPos.localPosition);
    }
}
