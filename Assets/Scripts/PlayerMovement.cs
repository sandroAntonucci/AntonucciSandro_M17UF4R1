using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    [SerializeField] private float moveSpeed = 5f;

    private PlayerInput playerInput;

    private CharacterController characterController;

    private InputAction moveAction;
    private InputAction attackAction;

    private Vector3 currentMovement;

    private float verticalVelocity;

    public static event Action OnAttack;


    private void Awake()
    {
        characterController = GetComponent<CharacterController>(); 
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions.FindAction("Move");
        attackAction = playerInput.actions.FindAction("Attack");

        attackAction.performed += ctx => OnAttack?.Invoke();
    }

    private void Update()
    {

        // Gravity
        if (characterController.isGrounded)
        {
            verticalVelocity = -0.5f;
        }
        else
        {
            verticalVelocity += Physics.gravity.y * Time.deltaTime * 2;
        }

        currentMovement.y = verticalVelocity;
        currentMovement.x = moveAction.ReadValue<Vector2>().x * moveSpeed;
        currentMovement.z = moveAction.ReadValue<Vector2>().y * moveSpeed;

        characterController.Move(currentMovement * Time.deltaTime);

    }


}
