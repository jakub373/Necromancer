using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class EnemyMovement : MonoBehaviour
{
    public Transform player;
    public EnemyLineOfSightChecker LineOfSightChecker;
    public NavMeshTriangulation Triangulation;
    public float updateRate = 0.1f;
    private NavMeshAgent Agent;
    private AgentLinkMover linkMover;
    [SerializeField] private Animator Animator = null;
    private Enemy Enemy;

    public EnemyState DefaultState;
    public EnemyState _state;
    public EnemyState State
    {
        get
        {
            return _state;
        }
        set
        {
            OnStateChange?.Invoke(_state, value);
            _state = value;
        }
    }

    public delegate void StateChangeEvent(EnemyState oldState, EnemyState newState);
    public StateChangeEvent OnStateChange;
    public float IdleLocationRadius = 10f;
    public float IdleMovespeedMultiplier = 0.5f;
    public Vector3[] Waypoints = new Vector3[4];
    [SerializeField] private int WaypointIndex = 0;

    public const string IsWalking = "isWalking", Jump = "isJumping", Landed = "Landed";

    private Coroutine FollowCoroutine;

    private void Awake()
    {
        Enemy = GetComponent<Enemy>();
        Agent = GetComponent<NavMeshAgent>();
        linkMover = GetComponent<AgentLinkMover>();

        linkMover.OnLinkStart += HandleLinkStart;
        linkMover.OnLinkEnd += HandleLinkEnd;

        LineOfSightChecker.OnGainSight += HandleGainSight;
        LineOfSightChecker.OnLoseSight += HandleLoseSight;

        OnStateChange += HandleStateChange;
    }
    private void HandleGainSight(Player player)
    {
        State = EnemyState.Chase;
    }

    private void HandleLoseSight(Player player)
    {
        State = DefaultState;
    }
    private void OnDisable()
    {
        _state = DefaultState;
    }
    public void Spawn()
    {
        for(int i=0; i < Waypoints.Length; i++) 
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(Triangulation.vertices[Random.Range(0, Triangulation.vertices.Length)], out hit, 5f, Agent.areaMask))
            {
                Waypoints[i] = hit.position;
            }
        }

        OnStateChange?.Invoke(EnemyState.Spawn, DefaultState);
    }
    private void HandleLinkStart(OffMeshLinkMoveMethod MoveMethod)
    {
        if (MoveMethod == OffMeshLinkMoveMethod.NormalSpeed)
        {
            Animator.SetBool(IsWalking, true);
        }
        else if (MoveMethod != OffMeshLinkMoveMethod.Teleport)
        {
            Animator.SetTrigger(Jump);
        }
    }

    private void HandleLinkEnd(OffMeshLinkMoveMethod MoveMethod)
    {
        if(MoveMethod !=OffMeshLinkMoveMethod.Teleport && MoveMethod != OffMeshLinkMoveMethod.NormalSpeed)
        {
            Animator.SetTrigger(Landed);
        }
    }

    private void Update()
    {
        if (!Agent.isOnOffMeshLink)
        {
            Animator.SetBool(IsWalking, Agent.velocity.magnitude > 0.01f);
        }
    }
    private void HandleStateChange(EnemyState oldState, EnemyState newState)
    {
        if (oldState != newState)
        {
            if (FollowCoroutine != null)
            {
                StopCoroutine(FollowCoroutine);
            }

            if (oldState == EnemyState.Idle)
            {
                Agent.speed /= IdleMovespeedMultiplier;
                Agent.angularSpeed /= IdleMovespeedMultiplier;
                Enemy.baseSpeed = Agent.speed;
                Enemy.baseAngularSpeed = Agent.angularSpeed;
            }

            switch (newState)
            {
                case EnemyState.Idle:
                    FollowCoroutine = StartCoroutine(DoIdleMotion());
                    break;

                case EnemyState.Patrol:
                    FollowCoroutine = StartCoroutine(DoPatrolMotion());
                    break;

                case EnemyState.Chase:
                    FollowCoroutine = StartCoroutine(FollowTarget());
                    break;
            }
        }
    }
    private IEnumerator DoIdleMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        Agent.speed *= IdleMovespeedMultiplier;
        Agent.angularSpeed *= IdleMovespeedMultiplier;
        Enemy.baseSpeed = Agent.speed;
        Enemy.baseAngularSpeed = Agent.angularSpeed;
        while (true)
        {
            if (!Agent.enabled || !Agent.isOnNavMesh)
            {
                yield return wait;
            }
            else if (Agent.remainingDistance <= Agent.stoppingDistance)
            {
                Vector2 point = Random.insideUnitSphere * IdleLocationRadius;
                NavMeshHit hit;

                if (NavMesh.SamplePosition(Agent.transform.position + new Vector3(point.x, 0, point.y), out hit, 5f, Agent.areaMask))
                {
                    Agent.SetDestination(hit.position);
                }
            }

            yield return wait;
        }
    }
    private IEnumerator DoPatrolMotion()
    {
        WaitForSeconds wait = new WaitForSeconds(updateRate);

        yield return new WaitUntil(() => Agent.enabled && Agent.isOnNavMesh);
        Agent.SetDestination(Waypoints[WaypointIndex]);

        while (true)
        {
            if(Agent.isOnNavMesh && Agent.enabled && Agent.remainingDistance <= Agent.stoppingDistance)
            {
                WaypointIndex++;

                if (WaypointIndex >= Waypoints.Length)
                {
                    WaypointIndex = 0;
                }

                Agent.SetDestination(Waypoints[WaypointIndex]);
            }

            yield return wait;
        }
    }

    private IEnumerator FollowTarget()
    {
        WaitForSeconds Wait = new WaitForSeconds(updateRate);

        while (gameObject.activeSelf)
        {
            if (Agent.enabled)
            {
                Agent.SetDestination(player.transform.position);
            }
            yield return Wait;
        }
    }

    private void OnDrawGizmosSelected()
    {
        for(int i = 0; i < Waypoints.Length; i++)
        {
            Gizmos.DrawWireSphere(Waypoints[i], 0.25f);
            if (i + 1 < Waypoints.Length)
            {
                Gizmos.DrawLine(Waypoints[i], Waypoints[i + 1]);
            }
            else
            {
                Gizmos.DrawLine(Waypoints[i], Waypoints[0]);
            }
        }
    }
}