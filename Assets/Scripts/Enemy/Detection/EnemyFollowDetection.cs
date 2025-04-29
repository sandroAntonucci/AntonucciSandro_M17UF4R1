using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFollowDetection : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private GameObject playerObject;

    private Coroutine playerDetectedWaitTime;

    public float fieldOfView = 120f;

    private void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.playerInRange = true;

            if (DetectPlayer())
            {
                enemy.playerDetected = true;

                if (playerDetectedWaitTime != null)
                {
                    StopCoroutine(playerDetectedWaitTime);
                    playerDetectedWaitTime = null;
                }
            }
            else if (enemy.playerDetected && playerDetectedWaitTime == null)
            {
                playerDetectedWaitTime = StartCoroutine(PlayerDetectedWaitTime());
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            enemy.playerInRange = false;

            if (playerDetectedWaitTime != null)
            {
                StopCoroutine(playerDetectedWaitTime);
                playerDetectedWaitTime = null;
            }
            enemy.playerDetected = false;
        }
    }

    private IEnumerator PlayerDetectedWaitTime()
    {
        yield return new WaitForSeconds(3f);
        enemy.playerDetected = false;
        playerDetectedWaitTime = null;
    }


    public bool DetectPlayer()
    {

        /*
        Debug.DrawRay(transform.position, Quaternion.Euler(0, -fieldOfView / 2, 0) * transform.forward * 10, Color.blue);
        Debug.DrawRay(transform.position, Quaternion.Euler(0, fieldOfView / 2, 0) * transform.forward * 10, Color.blue);
        */

        Vector3 playerDirection = (playerObject.transform.position - transform.position).normalized;

        float angle = Vector3.Angle(transform.forward, playerDirection);

        if (angle < fieldOfView / 2)
        {

            float distanceToPlayer = Vector3.Distance(transform.position, playerObject.transform.position);

            RaycastHit hit;

            if (Physics.Raycast(transform.position, playerDirection, out hit))
            {

                //Debug.DrawRay(transform.position, playerDirection * distanceToPlayer, Color.red);
                if (hit.collider.CompareTag("Player"))
                {
                    return true;
                }
            }

        }

        return false;
    }

}