using System;
using Unity.VisualScripting.Dependencies.Sqlite;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    // Start is called before the first frame update

    private CharacterController _characterController;
    [SerializeField] private float _speed;
    


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>(); 
    }

    // Update is called once per frame
    void Update()
    {
        var horizontalMovement = Input.GetAxis("Horizontal");
        var verticalMovement = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalMovement, 0, verticalMovement);

        _characterController.Move(movementDirection * _speed * 0.01f);
        
       transform.LookAt(transform.position + movementDirection);
    }
}
