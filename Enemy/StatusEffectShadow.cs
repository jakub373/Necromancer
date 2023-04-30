using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Status Effect Shadow", menuName = "ScriptableObject/Status Effect/Status Effect Manager Shadow")]
public class StatusEffectShadow : StatusEffectManagerScriptableObject
{
    public float waitForTime, waitForImmobilizeTime, slowTime, immobilizeTime, stunTime, burnTime;

    private WaitForSeconds waitFor, waitForImmobilize, burnTickTimer;
    private WaitUntil countDown;
    
    private IEnumerator slowStatic = null;
    public override void StartSettings(Enemy Enemy)
    {
        base.StartSettings(Enemy);
        countDown = new WaitUntil(Enemy.CountDownFreeze);
        waitFor = new WaitForSeconds(waitForTime);
        waitForImmobilize = new WaitForSeconds(waitForImmobilizeTime);
        burnTickTimer = new WaitForSeconds(burnTime);
    }
    public override void ApplySlow(Enemy Enemy)
    {
        Enemy.isSlowed = true;
        Enemy.durationImage1.fillAmount = 0;
        Enemy.StopCoroutine(slowStatic);
    }

    public override void ApplySlow2(Enemy Enemy) 
    {
        slowStatic = SlowStatic(Enemy); 
        Enemy.StartCoroutine(slowStatic); 
    }
    private IEnumerator SlowStatic(Enemy Enemy)
    {
        Enemy.waitForIt = true;
        yield return waitFor;
        Enemy.isSlowedStatic = true;
        Enemy.waitForIt = false;
        Enemy.durationImage1.fillAmount = 1;
        Enemy.durationImage2.fillAmount = 0;
        yield return waitForImmobilize;
        Enemy.StopCoroutine(slowStatic);
        Enemy.StartCoroutine(Immobilize(Enemy));
    }
    private IEnumerator Immobilize(Enemy Enemy)
    {
        //Static
        Enemy.isSlowedStatic = false;
        Enemy.isFreezedStatic = true;
        yield return countDown;
        //Stabil
        Enemy.isFreezedStatic = false;
        Enemy.isFreezed = true;
        Enemy.durationImage2.fillAmount = 0;
    }

    public override void ApplyBurn(int ticks, Enemy Enemy)
    {
        if (Enemy.dotTickTimers.Count <= 0)
        {
            Enemy.dotTickTimers.Add(ticks);
            Enemy.StartCoroutine(Burn(Enemy));
        }
        else {
            Enemy.dotTickTimers.Add(ticks);
            Enemy.durationImage3.fillAmount = 0;
        }
    }
    private IEnumerator Burn(Enemy Enemy)
    {
        Enemy.durationImage3.fillAmount = 0;
        Enemy.isBurned = true;

        while (Enemy.dotTickTimers.Count > 0)
        {
            yield return burnTickTimer;
            for (int i = 0; i < Enemy.dotTickTimers.Count; i++)
            { Enemy.dotTickTimers[i]--; }
            Enemy.TakeDamage(Damage);
            Enemy.dotTickTimers.RemoveAll(i => i == 0);
        }
    }

    public override void ApplyStun(float time, Enemy Enemy)
    {
        stunTime = time;
        Enemy.durationImage4.fillAmount = 0;
        Enemy.isStuned = true;
    }

    public override void DebuffTimers(Enemy Enemy)
    {
        base.DebuffTimers(Enemy);

        HandleFreeze(Enemy);
        HandleSlow(Enemy);
        HandleStun(Enemy);
        HandleBurn(Enemy);
    }
    private void HandleFreeze(Enemy Enemy)
    {
        if (Enemy.isFreezed || Enemy.isFreezedStatic)
        {
            Enemy.Agent.enabled = false;
            Enemy.Movement.enabled = false;
            Enemy.icon2.SetActive(true);
        }
        else
        {
            Enemy.Agent.enabled = true;
            Enemy.Movement.enabled = true;
            Enemy.icon2.SetActive(false);
        }

        if (Enemy.isFreezed)
        {
            Enemy.durationImage2.fillAmount += 1 / immobilizeTime * Time.deltaTime;
            if (Enemy.durationImage2.fillAmount >= 1)
            {
                Enemy.durationImage2.fillAmount = 1;
                Enemy.isFreezed = false;
            }
        }
    }

    private void HandleSlow(Enemy Enemy)
    {
        if (!Enemy.isSlowedStatic && !Enemy.waitForIt && slowStatic != null) { Enemy.StopCoroutine(slowStatic); }

        if (Enemy.isSlowed || Enemy.isSlowedStatic)
        {
            Enemy.icon1.SetActive(true);
            Enemy.Agent.speed = Enemy.baseSpeed * 0.5f;
            Enemy.Agent.angularSpeed = Enemy.baseAngularSpeed * 0.5f;
        }
        else
        {
            Enemy.icon1.SetActive(false);
            Enemy.Agent.speed = Enemy.baseSpeed;
            Enemy.Agent.angularSpeed = Enemy.baseAngularSpeed;
        }
        if (Enemy.isSlowedStatic)
        {
            Enemy.durationImage1.fillAmount -= 1 / waitForImmobilizeTime * Time.deltaTime;
            if (Enemy.durationImage1.fillAmount <= 0)
            {
                Enemy.durationImage1.fillAmount = 0;
            }
        }

        if (Enemy.isSlowed)
        {
            Enemy.durationImage1.fillAmount += 1 / slowTime * Time.deltaTime;
            if (Enemy.durationImage1.fillAmount >= 1)
            {
                Enemy.durationImage1.fillAmount = 1;
                Enemy.isSlowed = false;
            }
        }
    }
    private void HandleStun(Enemy Enemy)
    {
        if (Enemy.isStuned)
        {
            Enemy.Agent.enabled = false;
            Enemy.Movement.enabled = false;

            Enemy.icon4.SetActive(true);
            Enemy.AttackRadius.gameObject.SetActive(false);
            Enemy.Animator.SetLayerWeight(1, 0);
            Enemy.Animator.SetBool(IsStuned, true);

            Enemy.durationImage4.fillAmount += 1 / stunTime * Time.deltaTime;
            if (Enemy.durationImage4.fillAmount >= 1)
            {
                Enemy.durationImage4.fillAmount = 1;
                Enemy.isStuned = false;
            }
        }
        else
        {
            Enemy.Agent.enabled = true;
            Enemy.Movement.enabled = true;
            Enemy.icon4.SetActive(false);
            Enemy.AttackRadius.gameObject.SetActive(true);
            Enemy.Animator.SetBool(IsStuned, false);
        }
    }
    private void HandleBurn(Enemy Enemy)
    {
        if (Enemy.isBurned)
        {
            Enemy.icon3.SetActive(true);
            Enemy.durationImage3.fillAmount += 1 / (5f * burnTime) * Time.deltaTime;
            if (Enemy.durationImage3.fillAmount >= 1)
            {
                Enemy.durationImage3.fillAmount = 1;
                Enemy.isBurned = false;
                Enemy.icon3.SetActive(false);
            }
        }
        else { EnemyList.BurnDeregister(Enemy.gameObject); Enemy.icon3.SetActive(false); }
    }
}
