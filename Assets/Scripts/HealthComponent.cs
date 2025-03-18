using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{

    [SerializeField] private float maxHealth = 100f;

    private float currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        PlayerMovement.OnAttack += TakeDamage;
    }

    private void OnDisable()
    {
        PlayerMovement.OnAttack -= TakeDamage;
    }

    public void TakeDamage()
    {
        currentHealth -= 10;

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        Destroy(gameObject);
    }

}
