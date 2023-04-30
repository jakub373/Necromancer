using UnityEngine;

public class ReapersUrnEnemy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (EnemyList.enemies.Contains(other.gameObject))
        {
            EnemyList.PetRegisterEnemy(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (EnemyList.enemies.Contains(other.gameObject))
        {
            EnemyList.PetDeregisterEnemy(other.gameObject);
        }
    }
}