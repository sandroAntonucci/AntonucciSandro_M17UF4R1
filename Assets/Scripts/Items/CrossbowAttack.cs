using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CrossbowAttack : MonoBehaviour
{

    [SerializeField] private GameObject ArrowPrefab;

    public Transform shootPosition;

    public GameObject arrowPosition;

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
        arrowPosition.GetComponent<MeshRenderer>().enabled = false;
        while (attackCooldown < 1f)
        {
            attackCooldown += Time.deltaTime;
            yield return null;
        }
        arrowPosition.GetComponent<MeshRenderer>().enabled = true;
        attackCooldown = 1f;
    }

    private void Attack()
    {
        // Spawns the arrow with the current global rotation of this GameObject
        GameObject arrow = Instantiate(ArrowPrefab, shootPosition.position, arrowPosition.transform.rotation);

        arrow.layer = LayerMask.NameToLayer("Default");

        arrow.AddComponent<Rigidbody>();

        Rigidbody rb = arrow.GetComponent<Rigidbody>();

        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        rb.AddForce(transform.right * -25f, ForceMode.Impulse);

        Destroy(arrow, 5f);
    }
}
