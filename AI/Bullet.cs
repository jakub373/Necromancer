using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : PoolableObject
{
    public float autoDestroyTime = 5f;
    public float MoveSpeed = 2f;
    public float damage = 5;
    public Rigidbody rigid;
    protected Transform target;

    protected const string DISABLE_METHOD_NAME = "Disable";

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
    }
    protected virtual void OnEnable()
    {
        CancelInvoke(DISABLE_METHOD_NAME);
        Invoke(DISABLE_METHOD_NAME, autoDestroyTime);
    }
    public virtual void Spawn(Vector3 forward, float damage, Transform target)
    {
        this.damage = damage;
        this.target = target;

       rigid.AddForce(forward * MoveSpeed, ForceMode.VelocityChange);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable;

        if (other.TryGetComponent<IDamageable>(out damageable))
        {
            damageable.TakeDamage(damage);
        }

        Disable();
    }

    protected virtual void Disable()
    {
        CancelInvoke(DISABLE_METHOD_NAME);
        rigid.velocity = Vector3.zero;
        gameObject.SetActive(false);
    }
}