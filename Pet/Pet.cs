using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent), typeof(AgentLinkMover))]
public class Pet : PoolableObject, IDamageable
{
    public PetScriptableObject PetScript;
    public PetAttackRadius AttackRadius;
    public Animator Animator;
    public PetMovement Movement;
    private Coroutine LookCoroutine;
    private StatusShadow statusPlayer;
    private HealthBar playerHealthBar;

    public float health, maxHealth, petMod;
    private const string ATTACK_TRIGGER = "Attack", Dead = "Dead", SKILL_TRIGGER = "Skill";
    public int AttackCount;
    public int Level;
    public float UseTime;
    public float Cooldown;

    private void Awake()
    {
        statusPlayer = GameObject.Find("Inumar").GetComponent<StatusShadow>();
        playerHealthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        AttackRadius.OnAttack += OnAttack;
        Movement = GetComponent<PetMovement>();
    }
    public virtual void OnAttack(IDamageable damageable)
    {
        Animator.SetLayerWeight(1, 1);
        
        if (UseTime + Cooldown < Time.time)
        {
            Animator.SetTrigger(SKILL_TRIGGER);
            
            AttackRadius.IsActivating = true;
        }
        else
        {
            int randomV = Random.Range(1, AttackCount + 1);
            Animator.SetTrigger(ATTACK_TRIGGER + randomV);
        }
       
        if (LookCoroutine != null)
        {
            StopCoroutine(LookCoroutine);
        }

        LookCoroutine = StartCoroutine(LookAt(damageable.GetTransform()));
    }
    private IEnumerator LookAt(Transform target)
    {
        Quaternion lookRotation = Quaternion.LookRotation(target.position - transform.position);
        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, time);
            time += Time.deltaTime * 2;
            yield return null;
        }
    }
    private void OnEnable()
    {
        PetScript.SetupPet(this);
        health = maxHealth;   
    }
    void Update()
    {
        if (Keyboard.current.kKey.wasPressedThisFrame) 
        {
            TakeDamage(100); 
        }
    }
    public void TakeDamage(float amount)
    {
        health -= amount - (amount * petMod);

        if (statusPlayer != null)
        {
            statusPlayer.health -= amount * petMod;
            playerHealthBar.SetHealth(statusPlayer.health);
        }

        foreach (GameObject pet in statusPlayer.pets)
        {
            int i = statusPlayer.pets.IndexOf(pet);
            if (pet == this.gameObject) statusPlayer.petBars[i].SetHealth(health);
        }

        if (health >= maxHealth) { health = maxHealth; }
        if (health <= 0f) { Die(); }
    }
    public void TakeHeal(float amount)
    {
        health += amount;
        if (health >= maxHealth) health = maxHealth;
    }
    public void Die()
    {
        Movement.Agent.enabled = false;
        AttackRadius.gameObject.SetActive(false);
        Animator.SetTrigger(Dead);
        statusPlayer.RemovePet(this.gameObject);
        statusPlayer.CheckDeadPets();
    }
    public void SetupDie()
    {
        statusPlayer.RemovePet(this.gameObject);
        this.gameObject.SetActive(false);
    }
    public void SwitchDie()
    {
        statusPlayer.RemovePetOnSwitch(this.gameObject);
        this.gameObject.SetActive(false);
    }
    public Transform GetTransform()
    {
        return transform;
    }
}