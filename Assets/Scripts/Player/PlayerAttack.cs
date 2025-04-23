using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    public bool hasKnife = false;

    public bool hasCrossBow = false;

    public float mouseSensitivity = 100f;
    float xRotation = 0f;
    float yRotation = 0f;

    public Transform playerBody;
    public Transform bowTransform; 

    public GameObject knifeOne;
    public GameObject knifeTwo;

    public GameObject crossBow;

    public GameObject thirdPersonCamera;

    private Animator anim;

    PlayerInput playerInputActions;

    InputAction attackAction;
    InputAction aimAction;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        playerInputActions = GetComponent<PlayerInput>();
        attackAction = playerInputActions.actions["Attack"];
        aimAction = playerInputActions.actions["Aim"];
    }

    private void OnEnable()
    {
        attackAction.Enable();

        attackAction.performed += ctx =>
        {
            if (hasKnife) anim.SetBool("isAttacking", true);
        };
        attackAction.canceled += ctx =>
        {
            anim.SetBool("isAttacking", false);
        };

        aimAction.Enable();

        aimAction.performed += ctx =>
        {
            if (hasCrossBow)
            {
                anim.SetBool("isAiming", true);
                StartCoroutine(TransitionCamera());
            }
        };
        aimAction.canceled += ctx =>
        {
            thirdPersonCamera.SetActive(true);
            anim.SetBool("isAiming", false);
        };

    }

    private void OnDisable()
    {
        attackAction.Disable();

        attackAction.performed -= ctx =>
        {
            if (hasKnife) anim.SetBool("isAttacking", true);
        };
        attackAction.canceled -= ctx =>
        {
            anim.SetBool("isAttacking", false);
        };

        aimAction.Disable();

        aimAction.performed -= ctx =>
        {
            if (hasCrossBow)
            {
                anim.SetBool("isAiming", true);
                StartCoroutine(TransitionCamera());
            }
        };

        aimAction.canceled -= ctx =>
        {
            thirdPersonCamera.SetActive(true);
            anim.SetBool("isAiming", false);
        };
    }

    public void ShowKnives()
    {
        knifeOne.SetActive(true);
        knifeTwo.SetActive(true);
    }

    public void HideKnives()
    {
        knifeOne.SetActive(false);
        knifeTwo.SetActive(false);
    }

    public void ShowCrossBow()
    {
        crossBow.SetActive(true);
    }

    private void Update()
    {
        if (hasCrossBow && anim.GetBool("isAiming"))
        {
            //AimAlignToCamera();
            UpdateCameraLook();
        }
    }

    public IEnumerator TransitionCamera()
    {
        yield return new WaitForSeconds(0.5f);
        thirdPersonCamera.SetActive(false);

        Vector3 globalTransform = crossBow.transform.position;

        crossBow.transform.SetParent(gameObject.transform);

        crossBow.transform.position = globalTransform;
    }


    private void UpdateCameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Clamp pitch

        yRotation += mouseX; // Add this line if not using playerBody for yaw

        // Apply both pitch and yaw
        bowTransform.rotation = Quaternion.Euler(xRotation, yRotation, 0f);
    }
}
