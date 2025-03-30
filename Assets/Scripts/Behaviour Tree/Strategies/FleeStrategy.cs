using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleeStrategy : IStrategy
{
    readonly Transform entity;
    readonly NavMeshAgent agent;
    readonly float fleeDistance = 6;
    readonly float safeDistance = 6f; 

    public FleeStrategy(Transform entity, NavMeshAgent agent)
    {
        this.entity = entity;
        this.agent = agent;
    }

    public Node.Status Process()
    {
        Transform player = entity.GetComponent<Enemy>().player.transform;

        float distanceToPlayer = Vector3.Distance(entity.position, player.position);

        if (distanceToPlayer > safeDistance)
        {
            agent.ResetPath();
            return Node.Status.Success;
        }

        Vector3 direction = (entity.position - player.position).normalized;
        Vector3 fleeTarget = entity.position + direction * fleeDistance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(fleeTarget, out hit, fleeDistance, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
            return Node.Status.Running;
        }

        return Node.Status.Failure;
    }

    public Node.Status Reset()
    {
        return Node.Status.Success;
    }
}
