using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private List<Transform> waypoints = new();

    NavMeshAgent agent;
    BehaviourTree tree;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree("Hero");
        tree.AddChild(new Leaf("Patrol", new PatrolStrategy(transform, agent, waypoints)));
    }

    private void Update()
    {
        tree.Process();
    }
}
