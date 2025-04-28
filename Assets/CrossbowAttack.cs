using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossbowAttack : MonoBehaviour
{

    [SerializeField] private GameObject ArrowPrefab;

    public Transform shootPosition;

    public bool canAttack;

    private float attackCooldown = 1f;

    public PlayerInput playerInputActions;

    InputAction attackAction;

    private void Awake()
    {
        attackAction = playerInputActions.actions["Attack"];
    }

    private void OnEnable()
    {
        attackAction.Enable();

        attackAction.performed += ctx =>
        {
            if (canAttack && attackCooldown == 1f)
            {
                Attack();
                StartCoroutine(AttackCooldown());
            }

        };
    }

    private void OnDisable()
    {
        attackAction.Disable();
    }

    private IEnumerator AttackCooldown()
    {

        attackCooldown = 0f;

        while (attackCooldown < 1f)
        {
            attackCooldown += Time.deltaTime;
            yield return null;
        }

        attackCooldown = 1f;
    }

    private void Attack()
    {
        GameObject arrow = Instantiate(ArrowPrefab, shootPosition.position, Quaternion.identity);

        arrow.transform.rotation = Quaternion.Euler(90, 90, 90);

        arrow.transform.forward = transform.forward;

        arrow.AddComponent<Rigidbody>();

        Rigidbody rb = arrow.GetComponent<Rigidbody>();
        rb.AddForce(transform.right * -20f, ForceMode.Impulse);

        Destroy(arrow, 5f);
    }
}
