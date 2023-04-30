using UnityEngine;
using UnityEngine.UI;

public abstract class InumarAbilitySystem_Shoot : MonoBehaviour
{
    [HideInInspector] public StatusShadow status;
    [HideInInspector] public CementaryGround_Shoot cementary;
    [HideInInspector] public ReapersFire_Shoot fire;
    [HideInInspector] public ReapersUrn_Shoot urn;

    [HideInInspector] public Image abilityImage;
    public GameObject prefab;

    [HideInInspector] public bool isCooldown = false;

    public float cooldown;

    public virtual void Awake()
    {
        status = GetComponent<StatusShadow>();
        cementary = GetComponent<CementaryGround_Shoot>();
        fire = GetComponent<ReapersFire_Shoot>();
        urn = GetComponent<ReapersUrn_Shoot>();
    }

    public virtual void AbilityCooldown()
    {
        if (!isCooldown) { return; }
        abilityImage.fillAmount -= 1 / cooldown * Time.deltaTime;

        if (abilityImage.fillAmount <= 0)
        {
            abilityImage.fillAmount = 0;
            isCooldown = false;
        }
    }

    public virtual void GlobalCooldownCheck()
    {
        if (!status.globalCooldown() || isCooldown) { return; }
        abilityImage.fillAmount -= 1 / 1.5f * Time.deltaTime;

        if (abilityImage.fillAmount > 0) { return; }
        abilityImage.fillAmount = 0;
    }
}
