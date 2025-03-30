using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttackDetection : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private GameObject playerObject;

    public float fieldOfView = 120f;

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.playerInAttackRange = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.playerInAttackRange = DetectPlayer();
        }
    }

    public bool DetectPlayer()
    {

        Vector3 playerDirection = (playerObject.transform.position - transform.position).normalized;

        float angle = Vector3.Angle(transform.forward, playerDirection);

        if (angle < fieldOfView / 2)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, playerDirection, out hit))
            {
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }

        }

        return false;
    }

}