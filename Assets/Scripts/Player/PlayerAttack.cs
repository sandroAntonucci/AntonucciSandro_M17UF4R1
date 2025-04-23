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

    public Transform playerBody;
    public Transform cameraTransform; 

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
                thirdPersonCamera.SetActive(false);
                anim.SetBool("isAiming", true);
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
                thirdPersonCamera.SetActive(false);
                anim.SetBool("isAiming", true);
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

    private void AimAlignToCamera()
    {
        Vector3 camForward = cameraTransform.forward;
        camForward.y = 0; // Keep the rotation horizontal only
        if (camForward != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(camForward);
            playerBody.rotation = Quaternion.Slerp(playerBody.rotation, targetRotation, Time.deltaTime * 10f); // Smooth rotation
        }
    }


    private void UpdateCameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); // Clamp pitch

        cameraTransform.localRotation = Quaternion.Euler(0f, 39f, 0f);
        playerBody.Rotate(Vector3.up * mouseX);
    }
}
