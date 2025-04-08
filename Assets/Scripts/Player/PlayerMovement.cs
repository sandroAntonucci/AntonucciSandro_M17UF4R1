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

    private bool isRunning;
    private bool isJumping;
    private bool isCrouching;

    private bool isCheering = false;

    private Coroutine CheeringCoroutine;

    Rigidbody rb;
    PlayerInput playerInputActions;

    InputAction moveAction;
    InputAction jumpAction;
    InputAction runAction;
    InputAction crouchAction;
    InputAction cheerAction;


    public float currentJumpTimer;

    private Animator anim;

    private void Awake()
    {
        playerInputActions = GetComponent<PlayerInput>();
        anim = GetComponent<Animator>();

        moveAction = playerInputActions.actions["Move"];
        jumpAction = playerInputActions.actions["Jump"];
        runAction = playerInputActions.actions["Run"];
        crouchAction = playerInputActions.actions["Crouch"];
        cheerAction = playerInputActions.actions["Cheer"];
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();
        runAction.Enable();
        crouchAction.Enable();
        cheerAction.Enable();

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

        crouchAction.performed += ctx =>
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
        };
        crouchAction.canceled += ctx =>
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
        };

        cheerAction.performed += ctx =>
        {
            if (CheeringCoroutine == null)
            {
                CheeringCoroutine = StartCoroutine(Cheer());
            }
            anim.SetBool("isCheering", true);
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
        crouchAction.performed -= ctx =>
        {
            isCrouching = true;
            anim.SetBool("isCrouching", true);
        };
        crouchAction.canceled -= ctx =>
        {
            isCrouching = false;
            anim.SetBool("isCrouching", false);
        };
        cheerAction.performed -= ctx =>
        {
            if (CheeringCoroutine != null)
            {
                StopCoroutine(CheeringCoroutine);
                CheeringCoroutine = null;
            }
            anim.SetBool("isCheering", false);
        };
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        grounded = Physics.SphereCast(transform.position - new Vector3(0, -(playerHeight / 2), 0), 0.05f, Vector3.down, out RaycastHit hit, playerHeight * 0.5f + 0.1f, whatIsGround);

        anim.SetBool("isGrounded", grounded);


        if (isCheering)
        {
            return;
        }

        
        if (grounded && isJumping && currentJumpTimer > 0.1f)
        {
            currentJumpTimer = 0;
            anim.SetBool("isJumping", false);
            isJumping = false;
        }   

        if((isJumping && rb.velocity.y < 0 && currentJumpTimer > 0) || rb.velocity.y < -2f)
        {
            anim.SetBool("isFalling", true);
        }
        else
        {
            anim.SetBool("isFalling", false);
        }

        SpeedControl();

        rb.drag = grounded ? groundDrag : 0;
    }

    private void FixedUpdate()
    {
        if (isCheering)
        {
            return;
        }
        MovePlayer();
    }

    private void MovePlayer()
    {

        moveDirection = orientation.forward * moveInput.y + orientation.right * moveInput.x;

        float currentSpeed = isRunning ? runSpeed : walkSpeed;

        // Crouching takes priority over running
        if (isCrouching)
        {
            currentSpeed /= 2;
        }

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
            StartCoroutine(JumpTimer());
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private IEnumerator JumpTimer()
    {
        while (isJumping)
        {
            currentJumpTimer += Time.deltaTime;
            yield return null;
        }
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private IEnumerator Cheer()
    {
        isCheering = true;
        yield return new WaitForSeconds(1.8f);
        anim.SetBool("isCheering", false);
        isCheering = false;
        CheeringCoroutine = null;
    }

}
