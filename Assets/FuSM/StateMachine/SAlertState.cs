using UnityEngine;
using System.Collections;

public class SAlertState : IEnemyState
{

    private readonly StaterPattern enemy;
    private float searchTime;

    public SAlertState(StaterPattern statePattern)
    {
        enemy = statePattern;
    }


    void IEnemyState.Update()
    {
        Look();
        Search();
    }

    public void OnTriggerEnter(Collider other)
    {
        
    }

    public void ToPatrolState()
    {
        enemy.currentState = enemy.patrolState;
        searchTime = 0;
    }

    public void ToAlertState()
    {
     
    }

    public void ToChaseState()
    {
        enemy.currentState = enemy.chaseState;
        searchTime = 0;
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

    private void Search()
    {
        enemy.meshRenderFlag.material.color = Color.yellow;
        enemy.navMeshAgent.Stop();
        enemy.transform.Rotate(0, enemy.searchingTurnSpeed * Time.deltaTime, 0);
        searchTime += Time.deltaTime;

        if(searchTime >= enemy.searchingDuration)
        {
            ToPatrolState();
        }
    }

}
