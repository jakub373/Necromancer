using System.Collections.Generic;
using UnityEngine;

public static class EnemyList
{
    public static List<GameObject> enemies = new List<GameObject>(), petEnemies = new List<GameObject>(), burnEnemies = new List<GameObject>();
    public static Dictionary<Collider, float> _table = new Dictionary<Collider, float>();

    public static void RegisterEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
    }
    public static void DeregisterEnemy(GameObject enemy)
    {
        enemies.Remove(enemy);
    }
    public static void PetRegisterEnemy(GameObject enemy)
    {
        petEnemies.Add(enemy);
    }
    public static void PetDeregisterEnemy(GameObject enemy)
    {
        petEnemies.Remove(enemy);
    }

    public static void BurnRegister(GameObject enemy)
    {
        burnEnemies.Add(enemy);
    }
    public static void BurnDeregister(GameObject enemy)
    {
        burnEnemies.Remove(enemy);
    }
}
