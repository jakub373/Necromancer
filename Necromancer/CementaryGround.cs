using System.Collections.Generic;
using UnityEngine;

public class CementaryGround : MonoBehaviour
{
    [SerializeField] private float damage, nextTick;
    public int stack;

    public Dictionary<Collider, float> _table = new Dictionary<Collider, float>();

    private void OnTriggerEnter(Collider other)
    {
        if (EnemyList.enemies.Contains(other.gameObject))
        { 
            other.gameObject.TryGetComponent(out Enemy Enemy);
            Enemy.Status.ApplySlow2(Enemy);
        }

        if (EnemyList.enemies.Contains(other.gameObject) && !_table.ContainsKey(other))
        {
            _table[other] = float.NegativeInfinity;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        float timer;
        if (!_table.TryGetValue(other, out timer)) return; //if not in table, it's not an enemy

        if (Time.time > timer)
        {
            _table[other] = Time.time + nextTick;
            other.gameObject.TryGetComponent(out Enemy Enemy);
            Enemy.TakeDamage(damage);
            stack++;
        }
    }

    private void OnDisable()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 20f);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent(out Enemy Enemy))
            {
                if (Enemy.isSlowedStatic) { Enemy.Status.ApplySlow(Enemy); Enemy.isSlowedStatic = false; }
                Enemy.isFreezedStatic = false;
                Enemy.waitForIt = false;
            }
        }
    }
}