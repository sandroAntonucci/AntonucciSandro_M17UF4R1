using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour, IDamageable
{

    [SerializeField] private int maxHealth = 100;

    private int currentHealth;

    float attackCooldown = 0.4f;
    bool canAttack = true;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    void Update()
    {

        if (Input.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            if (canAttack)
            {
                StartCoroutine(Attack());
            }
        }

    }

    private IEnumerator Attack()
    {

        canAttack = false;

        foreach (var enemy in FindObjectsOfType<Enemy>())
        {
            enemy.TakeDamage(20);
        }

        yield return new WaitForSeconds(attackCooldown);

        canAttack = true;

    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        Debug.Log("Player took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    public void Die()
    {
        Destroy(gameObject);
    }

}
