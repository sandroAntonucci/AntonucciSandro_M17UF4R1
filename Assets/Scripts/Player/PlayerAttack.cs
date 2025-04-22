using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{

    public bool hasKnife = false;

    public bool hasCrossBow = false;

    public GameObject knifeOne;
    public GameObject knifeTwo;

    public GameObject crossBow;

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
            if (hasKnife || hasCrossBow) anim.SetBool("isAttacking", true);
        };
        attackAction.canceled += ctx =>
        {
            anim.SetBool("isAttacking", false);
        };

        aimAction.Enable();

        aimAction.performed += ctx =>
        {
            if (hasCrossBow) anim.SetBool("isAiming", true);
        };
        aimAction.canceled += ctx =>
        {
            anim.SetBool("isAiming", false);
        };

    }

    private void OnDisable()
    {
        attackAction.Disable();

        attackAction.performed -= ctx =>
        {
            if (hasKnife || hasCrossBow) anim.SetBool("isAttacking", true);
        };
        attackAction.canceled -= ctx =>
        {
            anim.SetBool("isAttacking", false);
        };

        aimAction.Disable();

        aimAction.performed -= ctx =>
        {
            if (hasCrossBow) anim.SetBool("isAiming", true);
        };

        aimAction.canceled -= ctx =>
        {
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

}
