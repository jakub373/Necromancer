using UnityEngine;

[CreateAssetMenu(fileName = "Pet Attack Configuration", menuName = "ScriptableObject/Pet Attack Configuration")]
public class PetAttackScriptableObject : ScriptableObject
{
    [Header("Basic Cofings")]
    public bool IsRanged = false;
    public float Damage = 5;
    public float AttackRadius = 1.5f;
    public float AttackDelay = 1.5f;
    public bool SecondAttack = false;
    public float SkillDamage = 10f;
    public float Cooldown = 5f;

    [Header("Ranged Configs")]
    public Bullet BulletPrefab;
    public Vector3 BulletSpawnOffset = new Vector3(0, 1, 0);
    public LayerMask LineOfSightLayers;
    public float AttackSpeed = 1f;
    public float SpawnTime = 0f;    

    public PetAttackScriptableObject ScaleUpForLevel(ScalingScriptableObject Scaling, int Level)
    {
        PetAttackScriptableObject scaledUpConfiguration = CreateInstance<PetAttackScriptableObject>();

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
    public void SetupPet(Pet pet)
    {
        (pet.AttackRadius.Collider == null ? pet.AttackRadius.GetComponent<SphereCollider>() : pet.AttackRadius.Collider).radius = AttackRadius;
        pet.AttackRadius.AttackDelay = AttackDelay;
        pet.AttackRadius.Damage = Damage;
        pet.AttackRadius.SkillDamage = SkillDamage;
        pet.Cooldown = Cooldown;

        if (IsRanged)
        {
            PetRangedAttackRadius rangedAttackRadius = pet.AttackRadius.GetComponent<PetRangedAttackRadius>();
            rangedAttackRadius.BulletPrefab = BulletPrefab;
            rangedAttackRadius.BulletSpawnOffset = BulletSpawnOffset;
            rangedAttackRadius.layer = LineOfSightLayers;
            rangedAttackRadius.AttackSpeed = AttackSpeed;
            rangedAttackRadius.SpawnTime = SpawnTime;

            rangedAttackRadius.CreateBulletPool();
        }
    }
}
