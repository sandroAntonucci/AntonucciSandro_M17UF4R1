using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossBowItem : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerAttack playerAttack = other.GetComponent<PlayerAttack>();
            if (playerAttack != null)
            {
                playerAttack.hasCrossBow = true;
                playerAttack.ShowCrossBow();
                Destroy(gameObject);
            }
        }
    }

}
