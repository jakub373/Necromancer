using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyLineOfSightChecker : MonoBehaviour
{
    public SphereCollider coll;
    public float FieldOfView = 360f;
    public LayerMask LineOfSightLayers;

    public delegate void GainSightEvent(Player player);
    public GainSightEvent OnGainSight;
    public delegate void LoseSightEvent(Player player);
    public LoseSightEvent OnLoseSight;

    private Coroutine CheckForLineOfSightCoroutine;
    private void Awake()
    {
        coll = GetComponent<SphereCollider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player;
        if(other.TryGetComponent<Player>(out player))
        {
            if (!CheckLineOfSight(player))
            {
                CheckForLineOfSightCoroutine = StartCoroutine(CheckForLineOfSight(player));
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Player player;
        if(other.TryGetComponent(out player))
        {
            OnLoseSight?.Invoke(player);
            if (CheckForLineOfSightCoroutine != null)
            {
                StopCoroutine(CheckForLineOfSightCoroutine);
            }
        }
    }

    private bool CheckLineOfSight(Player player)
    {
        Vector3 dir = (player.transform.position - transform.position).normalized;
        if (Vector3.Dot(transform.forward, dir) >= Mathf.Cos(FieldOfView))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, dir, out hit, coll.radius, LineOfSightLayers))
            {
                if (player.transform.GetComponent<Player>() != null)
                {
                    OnGainSight?.Invoke(player);
                    return true;
                }
            }
        }

        return false;
    }

    private IEnumerator CheckForLineOfSight(Player player)
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);

        while (!CheckLineOfSight(player))
        {
            yield return wait;
        }
    }
}