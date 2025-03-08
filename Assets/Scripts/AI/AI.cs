using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class AI : Damageable
{
    public enum AIState 
    { 
        Patrol, 
        Chase, 
        Attack
    }

    [Header("AI")]
    public AIState currentState;

    [SerializeField] float sightRange = 3f;
    [SerializeField] float attackRange = 25f;
    //[SerializeField] float alertRadius = 20f;
    [SerializeField] float speed = 5f;
    [SerializeField] float rotationSpeed = 5f;
    [SerializeField] Transform[] patrolPoints;

    private NavMeshAgent agent;
    private Actor actor;

    [Header("Debug")]
    [SerializeField] protected int currentPatrolPoint = 0;
    [SerializeField] protected Transform target;

    private bool hasAlerted = false;

    //[Header("Sound Effects")]


    protected override void Initialize()
    {
        base.Initialize();

        BEAT_Manager.BEAT += OnBeat;
        agent = GetComponent<NavMeshAgent>();
        actor = GetComponent<Actor>();
        agent.speed = speed;
        currentState = AIState.Patrol;
    }

    void Update()
    {
        if (target == null)
        {
            Actor act = FindNearestEnemy();

            if (act == null)
            return;

            target = act.gameObject.transform;
        }

        switch (currentState)
        {
            case AIState.Patrol:
                if (CanSeeTarget())
                {
                    Alert();
                    ChangeState(AIState.Chase);
                }
                Patrol();
                break;

            case AIState.Chase:
                ChaseTarget();
                if (IsInAttackRange() && CanSeeTarget())
                ChangeState(AIState.Attack);
                break;

            case AIState.Attack:
                AttackTarget();
                if (!IsInAttackRange())
                ChangeState(AIState.Chase);
                break;
        }
    }

    void ChangeState(AIState newState)
    {
        currentState = newState;
    }
    protected Actor FindNearestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, sightRange);
        Actor nearestEnemy = null;
        float minSqrDistance = sightRange * sightRange; // Use squared distance for optimization
        Vector3 myPosition = transform.position;

        foreach (Collider col in colliders)
        {
            Actor act = col.GetComponent<Actor>();
            if (act == null || act == this || act.faction == actor.faction) continue;

            float sqrDistance = (act.transform.position - myPosition).sqrMagnitude;
            if (sqrDistance < minSqrDistance)
            {
                minSqrDistance = sqrDistance;
                nearestEnemy = act;
            }
        }

        return nearestEnemy;
    }

    // Patrol state
    void Patrol()
    {
        if (patrolPoints.Length == 0)
            return;

        agent.SetDestination(patrolPoints[currentPatrolPoint].position);

        if (Vector3.Distance(transform.position, patrolPoints[currentPatrolPoint].position) < 1f)
            currentPatrolPoint = (currentPatrolPoint + 1) % patrolPoints.Length;
    }
    void ChaseTarget()
    {
        agent.SetDestination(target.position);
        LookAtTarget();
    }
    void AttackTarget()
    {
        LookAtTarget();
    }

    // Utility functions
    bool IsInAttackRange()
    {
        return Vector3.Distance(transform.position, target.position) <= attackRange;
    }

    bool CanSeeTarget()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, (target.position - transform.position).normalized, out hit, sightRange))
        {
            Damageable tar = hit.transform.GetComponent<Damageable>();
            if (tar != null)
            {
                return true;
            }
        }
        return false;
    }
    private void LookAtTarget()
    {
        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
        }
    }
    void Alert()
    {
        if (hasAlerted) return;

        /*
        Collider[] nearbyEnemies = Physics.OverlapSphere(transform.position, alertRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider enemy in nearbyEnemies)
        {
            EnemyAI ai = enemy.GetComponent<EnemyAI>();
            if (ai != null && ai != this) // Avoid notifying itself
            {
                ai.ChangeState(AIState.Chase);
            }
        }
        hasAlerted = true; // Prevent re-alerting in the same event
        */
    }
    #region events

    public virtual void OnDestroy()
    {
        BEAT_Manager.BEAT -= OnBeat;
    }

    protected virtual void OnBeat() { }


    #endregion
}