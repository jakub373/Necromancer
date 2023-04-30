using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RangedAttackRadius : AttackRadius
{
    public NavMeshAgent Agent;
    public Bullet BulletPrefab;
    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask layer;
    private ObjectPool BulletPool;
    [SerializeField] private float spherecastRadius = 0.1f;
    private RaycastHit Hit;
    private IDamageable targetDamageable;
    private Bullet bullet;
    public float AttackSpeed;
    public float SpawnTime;

    public void CreateBulletPool()
    {
        if (BulletPool == null)
        {
            BulletPool = ObjectPool.CreateInstance(BulletPrefab, Mathf.CeilToInt((1 / AttackDelay) * BulletPrefab.autoDestroyTime));
        }
    }
    protected override IEnumerator Attack()
    {
        WaitForSeconds Wait = new WaitForSeconds(AttackDelay);
        WaitForSeconds Spawn = new WaitForSeconds(SpawnTime);

        yield return Wait;

        while (Damageables.Count > 0)
        {
            for (int i = 0; i < Damageables.Count; i++)
            {
                if (HasLineOfSightTo(Damageables[i].GetTransform()))
                {
                    targetDamageable = Damageables[i];
                    OnAttack?.Invoke(Damageables[i]);
                    Agent.enabled = false;
                    break;
                }
            }
            yield return Spawn;

            if (targetDamageable != null)
            {
                PoolableObject poolableObject = BulletPool.GetObject();
                if (poolableObject != null)
                {
                    bullet = poolableObject.GetComponent<Bullet>();

                    bullet.transform.position = transform.position + transform.forward + BulletSpawnOffset;
                    bullet.transform.rotation = Agent.transform.rotation;

                    Vector3 Direction = targetDamageable.GetTransform().position - bullet.transform.position + transform.up * 1.2f;
                    Direction.Normalize();
                    bullet.Spawn(Direction, Damage, targetDamageable.GetTransform());
                }
            }
            else
            {
                Agent.enabled = true; // no target in line of sight, keep trying to get closer
            }

            yield return Wait;

            if (targetDamageable == null || !HasLineOfSightTo(targetDamageable.GetTransform()))
            {
                Agent.enabled = true;
            }

            Damageables.RemoveAll(DisabledDamageables);
        }
        
        Animator.SetLayerWeight(1, 0);
        Agent.enabled = true;
        AttackCoroutine = null;
    }

    private bool HasLineOfSightTo(Transform Target)
    {
        if (Physics.SphereCast(transform.position + BulletSpawnOffset, spherecastRadius, ((Target.position + BulletSpawnOffset) - (transform.position + BulletSpawnOffset)).normalized, out Hit, Collider.radius, layer))
        {
            IDamageable damageable;
            if (Hit.collider.TryGetComponent<IDamageable>(out damageable))
            {
                return damageable.GetTransform() == Target;
            }
        }

        return false;
    }

    protected override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);

        if (AttackCoroutine == null)
        {
            Agent.enabled = true;
        }
    }
}