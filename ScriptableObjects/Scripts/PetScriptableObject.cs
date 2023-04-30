using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Pet Configuration", menuName = "ScriptableObject/Pet Configuration")]
public class PetScriptableObject : ScriptableObject
{
    public PetAttackScriptableObject AttackConfiguration;

    // Enemy Stats
    public float MaxHealth = 100;

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

    public void SetupPet(Pet pet)
    {
        pet.Movement.Agent.acceleration = Acceleration;
        pet.Movement.Agent.angularSpeed = AngularSpeed;
        pet.Movement.Agent.areaMask = AreaMask;
        pet.Movement.Agent.avoidancePriority = AvoidancePriority;
        pet.Movement.Agent.baseOffset = BaseOffset;
        pet.Movement.Agent.height = Height;
        pet.Movement.Agent.obstacleAvoidanceType = ObstacleAvoidanceType;
        pet.Movement.Agent.radius = Radius;
        pet.Movement.Agent.speed = Speed;
        pet.Movement.Agent.stoppingDistance = StoppingDistance;

        pet.AttackCount = AttackCount;
        pet.maxHealth = MaxHealth;

        AttackConfiguration.SetupPet(pet);
    }
}