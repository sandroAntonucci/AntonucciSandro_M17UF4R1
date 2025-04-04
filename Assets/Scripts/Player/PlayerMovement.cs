using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed;
    public float runSpeed;
    public float groundDrag;
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump = true;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool grounded;

    public Transform orientation;

    Vector2 moveInput;
    Vector3 moveDirection;
    bool isRunning;

    bool isJumping;

    Rigidbody rb;
    PlayerInput playerInputActions;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction runAction;

    private Animator anim;

    private void Awake()
    {
        playerInputActions = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        moveAction = playerInputActions.actions["Move"];
        jumpAction = playerInputActions.actions["Jump"];
        runAction = playerInputActions.actions["Run"];
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        runAction.Enable();

        jumpAction.performed += Jump;

        moveAction.performed += ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
            anim.SetBool("isWalking", true);
        };

        moveAction.canceled += ctx =>
        {
            moveInput = Vector2.zero;
            anim.SetBool("isWalking", false);
        };

        runAction.performed += ctx =>
        {
            isRunning = true;
            anim.SetBool("isRunning", true);
        };

        runAction.canceled += ctx =>
        {
            isRunning = false;
            anim.SetBool("isRunning", false);
        };
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        runAction.Disable();

        jumpAction.performed -= Jump;
        moveAction.performed -= ctx =>
        {
            moveInput = ctx.ReadValue<Vector2>();
            anim.SetBool("isWalking", true);
        };
        moveAction.canceled -= ctx =>
        {
            moveInput = Vector2.zero;
            anim.SetBool("isWalking", false);
        };
        runAction.performed -= ctx =>
        {
            isRunning = true;
            anim.SetBool("isRunning", true);
        };
        runAction.canceled -= ctx =>
        {
            isRunning = false;
            anim.SetBool("isRunning", false);
        };
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.SphereCast(transform.position - new Vector3(0, -(playerHeight / 2), 0), 0.3f, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.1f, whatIsGround);
        
        anim.SetBool("isGrounded", grounded);

        if(grounded && !isJumping)
        {
            anim.SetBool("isJumping", false);
            anim.SetBool("isFalling", false);
            isJumping = false;
        }

        if((isJumping && rb.velocity.y < 0) || rb.velocity.y < -2f)
        {
            anim.SetBool("isFalling", true);
        }

        SpeedControl();

        rb.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;
        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        if (grounded)
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f, ForceMode.Acceleration);
        else
            rb.AddForce(moveDirection.normalized * currentSpeed * 10f * airMultiplier, ForceMode.Acceleration);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        float maxSpeed = isRunning ? runSpeed : walkSpeed;

        if (flatVel.magnitude > maxSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * maxSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump(InputAction.CallbackContext context)
    {
        if (readyToJump && grounded)
        {
            readyToJump = false;
            rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            anim.SetBool("isJumping", true);
            isJumping = true;
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
