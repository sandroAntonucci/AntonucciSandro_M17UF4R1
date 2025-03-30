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
        if (currentIndex == patrolPoints.Count) currentIndex = 0;

        if (agent.remainingDistance < 0.1f)
        {
            var target = patrolPoints[currentIndex];

            agent.SetDestination(target.position);

            Vector3 targetPosition = new Vector3(target.position.x, entity.position.y, target.position.z);
            entity.LookAt(targetPosition);

            Quaternion currentRotation = entity.rotation;
            entity.rotation = Quaternion.Euler(0f, currentRotation.eulerAngles.y, 0f);

            currentIndex++;
        }

        return Node.Status.Running;
    }


}