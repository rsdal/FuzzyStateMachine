using UnityEngine;
using System.Collections;

public class SChaseState : IEnemyState
{
    private readonly StaterPattern enemy;
    private float searchTime;

    public SChaseState(StaterPattern statePattern)
    {
        enemy = statePattern;
    }


    void IEnemyState.Update()
    {
        Look();
        Chase();
    }

    public void OnTriggerEnter(Collider other)
    {

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

    }

    private void Look()
    {
        RaycastHit hit;
        Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;

        if (Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
            enemy.chaseTarget = hit.transform;
        else
            ToAlertState();
    }

    private void Chase()
    {
        enemy.meshRenderFlag.material.color = Color.red;
        enemy.navMeshAgent.destination = enemy.chaseTarget.position;
        enemy.navMeshAgent.Resume();
    }
}
