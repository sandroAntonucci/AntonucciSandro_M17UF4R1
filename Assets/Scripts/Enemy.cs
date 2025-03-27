using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{

    [SerializeField] private List<Transform> waypoints = new();
    [SerializeField] GameObject player;
    [SerializeField] GameObject player2;

    NavMeshAgent agent;
    BehaviourTree tree;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        tree = new BehaviourTree("Hero");
        //tree.AddChild(new Leaf("Patrol", new PatrolStrategy(transform, agent, waypoints)));

        Leaf isPlayerPresent = new Leaf("IsPlayerPresent", new Condition(() => player.activeSelf));
        Leaf moveToPlayer = new Leaf("MoveToPlayer", new ActionStrategy(() => agent.SetDestination(player.transform.position)));

        Sequence goToPlayer = new Sequence("GoToPlayer", 10);
        goToPlayer.AddChild(isPlayerPresent);
        goToPlayer.AddChild(moveToPlayer);

        Sequence goToPlayer2 = new Sequence("GoToPlayer2", 20);
        goToPlayer2.AddChild(new Leaf("IsPlayer2Present", new Condition(() => player2.activeSelf)));
        goToPlayer2.AddChild(new Leaf("MoveToPlayer2", new ActionStrategy(() => agent.SetDestination(player2.transform.position))));

        RandomSelector goToPlayers = new RandomSelector("GoToPlayers");
        goToPlayers.AddChild(goToPlayer2);
        goToPlayers.AddChild(goToPlayer);

        tree.AddChild(goToPlayers);
    }

    private void Update()
    {
        tree.Process();
    }
}
