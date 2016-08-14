using UnityEngine;
using System.Collections;

public class StaterPattern : MonoBehaviour {

    public float Distance;
    public float searchingTurnSpeed = 120f;
    public float searchingDuration = 4f;
    public float sightRange = 20f;
    public Transform eyes;
    public Vector3 offset = new Vector3(0, 0.5f, 0);
    public MeshRenderer meshRenderFlag;
    public Transform[] waypoints;

    private Transform Target;

    [HideInInspector]
    public Transform chaseTarget;
    [HideInInspector]
    public IEnemyState currentState;
    [HideInInspector]
    public SChaseState chaseState;
    [HideInInspector]
    public SAlertState alertState;
    [HideInInspector]
    public SPatrolState patrolState;
    [HideInInspector]
    public NavMeshAgent navMeshAgent;

    
    private float searchTurnSpeed, searchTurnDuration, sightRangeTurn, speed;

    void Awake()
    {
        chaseState = new SChaseState(this);
        alertState = new SAlertState(this);
        patrolState = new SPatrolState(this);
        navMeshAgent = GetComponent<NavMeshAgent>();
        speed = navMeshAgent.speed;
    }

	void Start () {
        currentState = patrolState;

        searchTurnSpeed = searchingTurnSpeed;
        searchTurnDuration = searchingDuration;
        sightRangeTurn = sightRange;

        Target = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	public void UpdatePatrol(float weight)
    {
        sightRange = sightRangeTurn * weight;
        navMeshAgent.speed = speed * weight;
    }

    public void UpdateChase(float weight)
    {
        sightRange = sightRangeTurn * weight;
        navMeshAgent.speed = speed * weight;
    }

    public void UpdateAlert(float weight)
    {
        searchingTurnSpeed = searchTurnSpeed * weight;
        searchingDuration = searchTurnDuration * weight;
        sightRange = sightRangeTurn * weight;
    }


	void Update () {
        currentState.Update();

        if (Target != null)
            Distance = Vector3.Distance(eyes.position, Target.position);
	}

    private void OnTriggerEnter(Collider other)
    {
        currentState.OnTriggerEnter(other);
    }

}
