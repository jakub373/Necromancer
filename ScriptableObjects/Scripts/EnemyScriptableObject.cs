using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName ="Enemy Configuration", menuName ="ScriptableObject/Enemy Configuration")]
public class EnemyScriptableObject : ScriptableObject
{
    public StatusEffectManagerScriptableObject Status;
    public Enemy Prefab;
    public AttackScriptableObject AttackConfiguration;
    public SkillScriptableObject[] Skills;

    // Enemy Stats
    public float MaxHealth = 100;

    // Movement Stats
    public EnemyState DefaultState;
    public float IdleLocationRadius = 4f;
    public float IdleMovespeedMultiplier = 0.5f;
    [Range(2, 10)] public int Waypoints = 4;
    public float LineOfSightRange = 6f;
    public float FieldOfView = 90f;

    // NavMeshAgent Configs
    public float AIUpdateInterval = 0.1f;

    public float Acceleration = 8;
    public float AngularSpeed = 120;
    // -1 means everything
    public int AreaMask = -1;
    public int AvoidancePriority = 50;
    public float BaseOffset = 0;
    public float Height = 2f;
    public ObstacleAvoidanceType ObstacleAvoidanceType = ObstacleAvoidanceType.LowQualityObstacleAvoidance;
    public float Radius = 0.5f;
    public float Speed = 3f;
    public float StoppingDistance = 0.5f;
    public int AttackCount = 1;
    public bool StackSlow;
    public bool StackBurnStatic;
    public bool StackBurn2;

    public EnemyScriptableObject ScaleUpForLevel(ScalingScriptableObject Scaling, int Level)
    {
        EnemyScriptableObject scaledUpEnemy = CreateInstance<EnemyScriptableObject>();

        scaledUpEnemy.name = name;
        scaledUpEnemy.Prefab = Prefab;

        scaledUpEnemy.AttackConfiguration = AttackConfiguration.ScaleUpForLevel(Scaling, Level);

        scaledUpEnemy.Skills = new SkillScriptableObject[Skills.Length];
        if (Skills.Length != 0)
        {
            for (int i = 0; i < Skills.Length; i++)
            {
                scaledUpEnemy.Skills[i] = Skills[i].ScaleUpForLevel(Scaling, Level);
            }
        }

        scaledUpEnemy.MaxHealth = Mathf.FloorToInt(MaxHealth * Scaling.HealthCurve.Evaluate(Level));

        scaledUpEnemy.DefaultState = DefaultState;
        scaledUpEnemy.IdleLocationRadius = IdleLocationRadius;
        scaledUpEnemy.IdleMovespeedMultiplier = IdleMovespeedMultiplier;
        scaledUpEnemy.Waypoints = Waypoints;
        scaledUpEnemy.LineOfSightRange = LineOfSightRange;
        scaledUpEnemy.FieldOfView = FieldOfView;

        scaledUpEnemy.AIUpdateInterval = AIUpdateInterval;
        scaledUpEnemy.Acceleration = Acceleration;
        scaledUpEnemy.AngularSpeed = AngularSpeed;

        scaledUpEnemy.AreaMask = AreaMask;
        scaledUpEnemy.AvoidancePriority = AvoidancePriority;

        scaledUpEnemy.BaseOffset = BaseOffset;
        scaledUpEnemy.Height = Height;
        scaledUpEnemy.ObstacleAvoidanceType = ObstacleAvoidanceType;
        scaledUpEnemy.Radius = Radius;
        scaledUpEnemy.Speed = Speed * Scaling.SpeedCurve.Evaluate(Level);
        scaledUpEnemy.StoppingDistance = StoppingDistance;
        scaledUpEnemy.AttackCount = AttackCount;
        scaledUpEnemy.Status = Status;
        scaledUpEnemy.StackSlow = StackSlow;
        scaledUpEnemy.StackBurnStatic = StackBurnStatic;
        scaledUpEnemy.StackBurn2 = StackBurn2;

        return scaledUpEnemy;
    }
    public void SetupEnemy(Enemy enemy)
    {
        enemy.Agent.acceleration = Acceleration;
        enemy.Agent.angularSpeed = AngularSpeed;
        enemy.Agent.areaMask = AreaMask;
        enemy.Agent.avoidancePriority = AvoidancePriority;
        enemy.Agent.baseOffset = BaseOffset;
        enemy.Agent.height = Height;
        enemy.Agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        enemy.Agent.radius = Radius;
        enemy.Agent.speed = Speed;
        enemy.Agent.stoppingDistance = StoppingDistance;

        enemy.Movement.updateRate = AIUpdateInterval;
        enemy.Movement.DefaultState = DefaultState;
        enemy.Movement.IdleMovespeedMultiplier = IdleMovespeedMultiplier;
        enemy.Movement.IdleLocationRadius = IdleLocationRadius;
        enemy.Movement.Waypoints = new Vector3[Waypoints];
        enemy.Movement.LineOfSightChecker.FieldOfView = FieldOfView;
        enemy.Movement.LineOfSightChecker.coll.radius = LineOfSightRange;
        enemy.Movement.LineOfSightChecker.LineOfSightLayers = AttackConfiguration.LineOfSightLayers;

        enemy.maxHealth = MaxHealth;
        enemy.Health = MaxHealth;
        enemy.EnemyHealthBar.SetMaxHealth(MaxHealth);
        enemy.baseSpeed = Speed;
        enemy.baseAngularSpeed = AngularSpeed;
        enemy.AttackCount = AttackCount;
        enemy.Status = Status;
        enemy.StackSlow = StackSlow;
        enemy.StackBurnStatic = StackBurnStatic;
        enemy.StackBurn2 = StackBurn2;

        if (!enemy.StackSlow) { enemy.stackSlow.gameObject.SetActive(false); }
        else { enemy.stackSlow.gameObject.SetActive(true); }

        if (!enemy.StackBurn2) { enemy.stackBurn2.gameObject.SetActive(false); }
        else { enemy.stackBurn2.gameObject.SetActive(true); }

        if (!enemy.StackBurnStatic) { enemy.stackBurnStatic.gameObject.SetActive(false); }
        else { enemy.stackBurnStatic.gameObject.SetActive(true); }

        Status.StartSettings(enemy);
        AttackConfiguration.SetupEnemy(enemy);
    }
}
