using UnityEngine;
using System.Collections;

public class SPatrolState : IEnemyState {

    private readonly StaterPattern enemy;
    private int nextWayPoint;

    public SPatrolState(StaterPattern statePattern)
    {
        enemy = statePattern;
    }

    void IEnemyState.Update()
    {
        Look();
        Patrol();
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            ToAlertState();
        }
    }

    public void ToPatrolState()
    {
        
    }

    public void ToAlertState()
    {
        enemy.currentState = enemy.alertState;
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
    }

    private void Look()
    {
        RaycastHit hit;
        if (Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
        {
            enemy.chaseTarget = hit.transform;
            ToChaseState();
        }
    }

    private void Patrol()
    {
        enemy.meshRenderFlag.material.color = Color.green;
        enemy.navMeshAgent.destination = enemy.waypoints[nextWayPoint].position;
        enemy.navMeshAgent.Resume();

        if(enemy.navMeshAgent.remainingDistance <= enemy.navMeshAgent.stoppingDistance && !enemy.navMeshAgent.pathPending)
            nextWayPoint = (nextWayPoint + 1) % enemy.waypoints.Length;
    }
}
