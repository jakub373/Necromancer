using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class ReapersFire_Shoot : InumarAbilitySystem_Shoot
{
    [SerializeField] private Animator animator;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float range, damage, heal;
    
    public override void Awake()
    {
        base.Awake();

        abilityImage = GameObject.Find("Skill2 BW").GetComponent<Image>();
        abilityImage.fillAmount = 0;
    }

    private void Update()
    {
        GlobalCooldownCheck();
        AbilityCooldown();
        if (status.setupState || cementary.groundCastUp || !Keyboard.current.qKey.wasPressedThisFrame 
            || isCooldown || status.globalCooldown()) { return; }
        Burn();
    }

    private void Burn()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = fpsCam.ScreenToWorldPoint(mousePos);

        Debug.DrawRay(attackPoint.position, mousePos - attackPoint.position, Color.blue);

        Vector3 dir = mousePos - attackPoint.position;
        dir.Normalize();

        RaycastHit hit;

        if (Physics.Raycast(attackPoint.position, dir, out hit, range, layer))
        {
            status.GlobalCooldown();
            if (!cementary.isCooldown) cementary.abilityImage.fillAmount = 1;
            if (!urn.isCooldown) urn.abilityImage.fillAmount = 1;

            isCooldown = true;
            abilityImage.fillAmount = 1;

            GameObject hitObject = hit.transform.gameObject;
            IDamageable damageable = hitObject.GetComponent<IDamageable>();

            if (damageable != null)
            {
                hitObject.TryGetComponent(out Enemy enemy);
                enemy.Status.ApplyBurn(5, enemy);
                EnemyList.BurnRegister(hitObject);
                damageable.TakeDamage(damage);
                animator.SetTrigger("ReaperFire");
            }
        }
    }
}