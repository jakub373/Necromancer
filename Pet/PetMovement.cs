using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class PetMovement : MonoBehaviour
{
    public Animator Animator;
    public NavMeshAgent Agent;
    public Pet Pet;
    public Transform Target;
    public GameObject urn;
    private AgentLinkMover Link;

    private const string isWalking = "isWalking", isJumping = "isJumping", Landed = "Landed";
    private float StoppingDistance;

    private void Awake()
    {
        Pet = GetComponent<Pet>();
        Animator = GetComponent<Animator>();
        Agent = GetComponent<NavMeshAgent>();
        Link = GetComponent<AgentLinkMover>();
    }
    private void OnEnable()
    {
        urn = GameObject.Find("UrnBody");
        Target = urn.transform;
        Link.OnLinkStart += HandleLinkStart;
        Link.OnLinkEnd += HandleLinkEnd;
        StoppingDistance = Agent.stoppingDistance;
    }
    void Update()
    {
        if (EnemyList.burnEnemies.Count == 0)
        {
            if (EnemyList.petEnemies.Count == 0)
            {
                Target = urn.transform;
            }
            else
            {
                Target = FindTarget();
            }
        }
        else
        {
            Target = FindTarget();
        }
    }
    private void LateUpdate()
    {
        if (Target.gameObject.activeSelf == false || Target == null) return;

        if (Target == urn.transform)
        {
            if (Distance() > StoppingDistance)
            {
                Agent.enabled = true;
                Agent.SetDestination(Target.position);
            }
            else { Agent.enabled = false; }
        }
        else
        {
            if (Distance() > ((Pet.AttackRadius.Collider.radius * 0.5f) - 2f))
            {
                Agent.enabled = true;
                Agent.SetDestination(Target.position);
            }
            else { Agent.enabled = false; }
        }

        Animator.SetBool(isWalking, Agent.velocity.magnitude > 0.01f);
    }
    public Transform FindTarget()
    {
        float distance = Mathf.Infinity;
        Vector3 position = transform.position;

        if (EnemyList.burnEnemies.Count != 0)
        {
            foreach (GameObject go in EnemyList.burnEnemies)
            {
                Vector3 diff = go.transform.position - position;
                float curDistance = diff.sqrMagnitude;
                if (curDistance < distance)
                {
                    Target = go.transform;
                    distance = curDistance;
                }
            }
            return Target;
        }
        else
        {
            foreach (GameObject go in EnemyList.petEnemies)
            {
                if (go == null) { return Target = urn.transform; }
                else
                {
                    Vector3 diff = go.transform.position - position;
                    float curDistance = diff.sqrMagnitude;
                    if (curDistance < distance)
                    {
                        Target = go.transform;
                        distance = curDistance;
                    }
                }
            }
            return Target;
        }

    }
    float Distance()
    {
        float distance = Vector3.Distance(this.transform.position, Target.transform.position);
        return distance;
    }
    private void HandleLinkStart(OffMeshLinkMoveMethod MoveMethod)
    {
        if (MoveMethod == OffMeshLinkMoveMethod.NormalSpeed)
        {
            Animator.SetBool(isWalking, true);
        }
        else if (MoveMethod != OffMeshLinkMoveMethod.Teleport)
        {
            Animator.SetTrigger(isJumping);
        }
    }
    private void HandleLinkEnd(OffMeshLinkMoveMethod MoveMethod)
    {
        if (MoveMethod != OffMeshLinkMoveMethod.Teleport && MoveMethod != OffMeshLinkMoveMethod.NormalSpeed)
        {
            Agent.speed = 0;
            Animator.SetTrigger(Landed);
            StartCoroutine(Stop());
        }
    }
    IEnumerator Stop()
    {
        yield return new WaitForSeconds(2f);
        Agent.speed = 3.5f;
    }
}