using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AttackRadius : MonoBehaviour
{
    public SphereCollider Collider;
    protected List<IDamageable> Damageables = new List<IDamageable>();
    public float Damage = 10f;
    public float AttackDelay = 0.5f;
    public delegate void AttackEvent(IDamageable Target);
    public AttackEvent OnAttack;
    protected Coroutine AttackCoroutine;
    public bool SecondAttack;
    [HideInInspector] public Animator Animator;
    [HideInInspector] public float ColliderRadius;

    protected virtual void Awake()
    {
        Collider = GetComponent<SphereCollider>();
        Animator = GetComponentInParent<Animator>();
        ColliderRadius = Collider.radius;
        Animator.SetLayerWeight(1, 0);
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Damageables.Add(damageable);
            if (AttackCoroutine == null)
            {
                AttackCoroutine = StartCoroutine(Attack());
                Animator.SetLayerWeight(1, 1);
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            Damageables.Remove(damageable);
            if (Damageables.Count == 0 && AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
                AttackCoroutine = null;
            }
            Animator.SetLayerWeight(1, 0);
        }
        Collider.radius = ColliderRadius;
    }

    protected virtual IEnumerator Attack()
    {
        WaitForSeconds Wait = new WaitForSeconds(AttackDelay);
        WaitForSeconds Wait2 = new WaitForSeconds(1f);
        yield return Wait;

        IDamageable closestDamageable = null;
        float closestDistance = float.MaxValue;

        while (Damageables.Count > 0)
        {
            for (int i = 0; i < Damageables.Count; i++)
            {
                Transform damageableTransform = Damageables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTransform.position);

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestDamageable = Damageables[i];
                }
            }

            if (closestDamageable != null)
            {
                OnAttack?.Invoke(closestDamageable);
                closestDamageable.TakeDamage(Damage);
                if (SecondAttack)
                {
                    yield return Wait2;
                    closestDamageable.TakeDamage(Damage);
                }
            }

            closestDamageable = null;
            closestDistance = float.MaxValue;

            yield return Wait;

            Damageables.RemoveAll(DisabledDamageables);
        }
        
        Animator.SetLayerWeight(1, 0);
        AttackCoroutine = null;
    }

    protected bool DisabledDamageables(IDamageable Damageable)
    {
        return Damageable != null && !Damageable.GetTransform().gameObject.activeSelf;
    }
}