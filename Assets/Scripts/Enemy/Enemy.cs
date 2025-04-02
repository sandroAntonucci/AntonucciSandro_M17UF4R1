using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IDamageable
{

    [SerializeField] private List<Transform> waypoints = new();
    [SerializeField] public GameObject player;

    [SerializeField] private int MaxHealth = 100;

    private int currentHealth;

    private bool canAttack = true;

    NavMeshAgent agent;
    BehaviourTree tree;

    public bool playerDetected = false;
    public bool playerInRange = false;
    public bool playerInAttackRange = false;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        currentHealth = MaxHealth;

        EnemyBehaviourTree();

    }

    private void Update()
    {
        tree.Process();
    }


    private void Attack()
    {
        if (canAttack)
        {
            if (player == null) return;
            //player.GetComponent<PlayerMovement>().TakeDamage(20);
            Debug.Log("Enemy attacked player");
            StartCoroutine(AttackCooldown());
        }
    }

    private IEnumerator AttackCooldown()
    {
        canAttack = false;

        yield return new WaitForSeconds(0.5f);

        canAttack = true;
    }


    private void EnemyBehaviourTree()
    {
        tree = new BehaviourTree("Knight");

        PrioritySelector AIBehaviour = new PrioritySelector("AIBehaviour");

        // FLEE FROM PLAYER

        Sequence fleeFromPlayer = new Sequence("FleeFromPlayer", 30);

        bool IsEnemyInDanger()
        {
            if (!playerInRange || currentHealth > MaxHealth / 2)
            {
                fleeFromPlayer.Reset();
                return false;
            }

            return true;
        }

        fleeFromPlayer.AddChild(new Leaf("IsPlayerDetected", new Condition(IsEnemyInDanger)));
        fleeFromPlayer.AddChild(new Leaf("FleeFromPlayer", new FleeStrategy(transform, agent)));

        AIBehaviour.AddChild(fleeFromPlayer);

        // ATTACK PLAYER

        Sequence attackPlayer = new Sequence("AttackPlayer", 20);

        bool IsPlayerInAttackRange()
        {
            if (!playerInAttackRange)
            {
                agent.isStopped = false;
                attackPlayer.Reset();
                return false;
            }

            agent.isStopped = true;
            return true;
        }

        attackPlayer.AddChild(new Leaf("IsPlayerInAttackRange", new Condition(IsPlayerInAttackRange)));
        attackPlayer.AddChild(new Leaf("AttackPlayer", new ActionStrategy(() => Attack())));

        AIBehaviour.AddChild(attackPlayer);


        // FOLLOW PLAYER

        Sequence followPlayer = new Sequence("FollowPlayer", 10);

        bool IsFollowingPlayer()
        {
            if (!playerDetected)
            {
                followPlayer.Reset();
                return false;
            }

            return true;
        }

        followPlayer.AddChild(new Leaf("IsFollowingPlayer", new Condition(IsFollowingPlayer)));
        followPlayer.AddChild(new Leaf("MoveToPlayer", new ActionStrategy(() => agent.SetDestination(player.transform.position))));

        AIBehaviour.AddChild(followPlayer);

        // PATROL

        Leaf patrol = new Leaf("Patrol", new PatrolStrategy(transform, agent, waypoints));

        AIBehaviour.AddChild(patrol);

        tree.AddChild(AIBehaviour);


    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Enemy took damage: " + damage);

        currentHealth -= damage;

        Debug.Log("Enemy health: " + currentHealth);

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
