using UnityEngine;

public class StatusEffectManagerScriptableObject : ScriptableObject
{
    public const string Dead = "Dead", IsStuned = "isStuned";
    public float Damage;

    public LayerMask DefaultLayer;
    public LayerMask EnemyLayer;

    //Settings
    public virtual void StartSettings(Enemy Enemy) { }
    public virtual void SetPlayerSettings(Enemy Enemy) 
    {
        Enemy.OnDie += HandleEnemyDeath; 
    }

    //Slows
    public virtual void ApplySlow(Enemy Enemy) { } //Rael, Inumar, Lesarin
    public virtual void ApplySlow2(Enemy Enemy) { } // Rael, Inumar

    //Burns
    public virtual void ApplyBurn(int ticks, Enemy Enemy) { } //Zazura, Lesarin Roots, Inumar, Sarin Purify
    public virtual void ApplyBurn2(int ticks, Enemy Enemy, Transform Transform) { } //Sarin Ashing
    public virtual void ApplyBurnStack(int ticks, Enemy Enemy, float Damage) { } //Lesarin
    public virtual void ApplyBurnStack2(int ticks, Enemy Enemy, float Damage) { } //Lesarin

    //Stuns/Freezes
    public virtual void ApplyStun(float time, Enemy Enemy) { } //Lesarin, Sarin, Inumar
    public virtual void ApplyFreeze(float ticks, Enemy Enemy) { } //Rael, Lesarin
    public virtual void DebuffTimers(Enemy Enemy) { }
    public virtual void HandleEnemyDeath(Enemy Enemy)
    {
        Enemy.Animator.SetLayerWeight(1, 0);
        Enemy.Animator.SetBool(Dead, true);
        Enemy.Agent.enabled = false;
        Enemy.Movement.enabled = false;
        Enemy.AttackRadius.gameObject.SetActive(false);
        Enemy.gameObject.layer = LayerMask.NameToLayer("Default");
        return;
    }
}
