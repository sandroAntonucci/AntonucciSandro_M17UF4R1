using Cinemachine;
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

    public Transform orientation;

    public Transform playerBody;

    public GameObject knifeOne;
    public GameObject knifeTwo;

    public GameObject crossBow;
    public GameObject visibleCrossBow;

    public GameObject thirdPersonCamera;

    private Animator anim;

    private Coroutine transitionCameraCoroutine;

    public CrossbowAttack crossbowAttack;

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
                transitionCameraCoroutine = StartCoroutine(TransitionCamera());
            }
        };
        aimAction.canceled += ctx =>
        {
            if (transitionCameraCoroutine != null)
            {
                StopCoroutine(transitionCameraCoroutine);
                transitionCameraCoroutine = null;
            }
            thirdPersonCamera.SetActive(true);
            UpdateCullingMaskNotAiming();
            crossbowAttack.canAttack = false;
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
                transitionCameraCoroutine = StartCoroutine(TransitionCamera());
            }
        };

        aimAction.canceled -= ctx =>
        {
            if (transitionCameraCoroutine != null)
            {
                StopCoroutine(transitionCameraCoroutine);
                transitionCameraCoroutine = null;
            }
            thirdPersonCamera.SetActive(true);
            UpdateCullingMaskNotAiming();
            crossbowAttack.canAttack = false;
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
        visibleCrossBow.SetActive(true);
    }

    private void Update()
    {
        if (hasCrossBow && anim.GetBool("isAiming"))
        {
            UpdateCameraLook();
        }
    }

    public IEnumerator TransitionCamera()
    {
        thirdPersonCamera.SetActive(false);

        yield return new WaitForSeconds(0.3f);

        UpdateCullingMaskAiming();

        crossbowAttack.canAttack = true;
    }

    private void UpdateCullingMaskAiming()
    {
        Camera cam = Camera.main;
        int playerLayer = LayerMask.NameToLayer("Player");
        int crossbowLayer = LayerMask.NameToLayer("CrossBow");

        // Si existen las capas "Player" y "Crossbow"
        if (playerLayer != -1 && crossbowLayer != -1)
        {
            // Quitamos la capa "Player" del culling mask de la cámara
            cam.cullingMask = cam.cullingMask & ~(1 << playerLayer);

            // Añadimos la capa "Crossbow" al culling mask de la cámara
            cam.cullingMask |= 1 << crossbowLayer;
        }
    }

    private void UpdateCullingMaskNotAiming()
    {

        Camera cam = Camera.main;
        int playerLayer = LayerMask.NameToLayer("Player");
        int crossbowLayer = LayerMask.NameToLayer("CrossBow");

        // Si existen las capas "Player" y "Crossbow"
        if (playerLayer != -1 && crossbowLayer != -1)
        {
            // Quitamos la capa "Player" del culling mask de la cámara
            cam.cullingMask = cam.cullingMask & ~(1 << crossbowLayer);

            // Añadimos la capa "Crossbow" al culling mask de la cámara
            cam.cullingMask |= 1 << playerLayer;
        }



    }



    void UpdateCameraLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        playerBody.Rotate(Vector3.up * mouseX);
        orientation.Rotate(Vector3.up * mouseX);

        yRotation -= mouseY;
        yRotation = Mathf.Clamp(yRotation, -90f, 90f);

        crossBow.transform.localRotation = Quaternion.Euler(0, orientation.eulerAngles.y + 90 , yRotation);
    }
}
