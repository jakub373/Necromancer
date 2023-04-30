using UnityEngine;

[CreateAssetMenu(fileName = "Attack Configuration", menuName = "ScriptableObject/Attack Configuration")]
public class AttackScriptableObject : ScriptableObject
{
    [Header("Basic Cofings")]
    public bool IsRanged = false;
    public float Damage = 5;
    public float AttackRadius = 1.5f;
    public float AttackDelay = 1.5f;
    public bool SecondAttack = false;

    [Header("Ranged Configs")]
    public Bullet BulletPrefab;
    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask LineOfSightLayers;
    public float AttackSpeed = 1f;
    public float SpawnTime = 0f;

    public AttackScriptableObject ScaleUpForLevel(ScalingScriptableObject Scaling, int Level)
    {
        AttackScriptableObject scaledUpConfiguration = CreateInstance<AttackScriptableObject>();

        scaledUpConfiguration.IsRanged = IsRanged;
        scaledUpConfiguration.Damage = Mathf.FloorToInt(Damage * Scaling.DamageCurve.Evaluate(Level));
        scaledUpConfiguration.AttackRadius = AttackRadius;
        scaledUpConfiguration.AttackDelay = AttackDelay;
        scaledUpConfiguration.SecondAttack = SecondAttack;

        scaledUpConfiguration.BulletPrefab = BulletPrefab;
        scaledUpConfiguration.BulletSpawnOffset = BulletSpawnOffset;
        scaledUpConfiguration.LineOfSightLayers = LineOfSightLayers;

        return scaledUpConfiguration;
    }

    public void SetupEnemy(Enemy enemy)
    {
        (enemy.AttackRadius.Collider == null ? enemy.AttackRadius.GetComponent<SphereCollider>() : enemy.AttackRadius.Collider).radius = AttackRadius;
        enemy.AttackRadius.AttackDelay = AttackDelay;
        enemy.AttackRadius.Damage = Damage;
        enemy.AttackRadius.SecondAttack = SecondAttack;
        

        if (IsRanged)
        {
            RangedAttackRadius rangedAttackRadius = enemy.AttackRadius.GetComponent<RangedAttackRadius>();

            rangedAttackRadius.BulletPrefab = BulletPrefab;
            rangedAttackRadius.BulletSpawnOffset = BulletSpawnOffset;
            rangedAttackRadius.layer = LineOfSightLayers;
            rangedAttackRadius.AttackSpeed = AttackSpeed;
            rangedAttackRadius.SpawnTime = SpawnTime;

            rangedAttackRadius.CreateBulletPool();
        }
    }
}
