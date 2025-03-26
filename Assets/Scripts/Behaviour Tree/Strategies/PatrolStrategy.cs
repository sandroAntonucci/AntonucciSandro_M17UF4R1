using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolStrategy : IStrategy
{

    readonly Transform entity;
    readonly NavMeshAgent agent;
    readonly List<Transform> patrolPoints;
    readonly float patrolSpeed;
    int currentIndex;

    public PatrolStrategy(Transform entity, NavMeshAgent agent, List<Transform> patrolPoints, float patrolSpeed = 4f)
    {
        this.entity = entity;
        this.agent = agent;
        this.patrolPoints = patrolPoints;
        this.patrolSpeed = patrolSpeed;
        currentIndex = 0;
    }

    public Node.Status Process()
    {
        if (currentIndex == patrolPoints.Count) return Node.Status.Success;

        if (agent.remainingDistance < 0.1f)
        {
            var target = patrolPoints[currentIndex];
            agent.SetDestination(target.position);
            entity.LookAt(target);

            currentIndex++;
        }

        return Node.Status.Running;
    }

    public void Reset() => currentIndex = 0;

}
