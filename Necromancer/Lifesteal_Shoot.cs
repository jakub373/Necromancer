using UnityEngine;
using UnityEngine.InputSystem;

public class Lifesteal_Shoot : MonoBehaviour
{
    //private Animator animator;
    //private AudioSource gunAudio;
    [SerializeField] private LayerMask layer;
    [SerializeField] private Animator animator;
    private StatusShadow Player;
    [SerializeField] private Transform attackPoint, particle;
    private CementaryGround_Shoot cementary;

    [SerializeField] private float minRadius, maxRadius, maxDistance, damage, heal, radius;
    private float nextFire, cd;

    private int animIDLifestealBackward;
    private void Awake()
    {
        Player = GetComponent<StatusShadow>();
        cementary = GetComponent<CementaryGround_Shoot>();
        animIDLifestealBackward = Animator.StringToHash("LifestealBackward");
    }

    private void Update()
    {
        if (cd < Time.time && radius > minRadius) { radius -= 0.01f; }
        if (Mouse.current.leftButton.wasReleasedThisFrame && animator.GetCurrentAnimatorStateInfo(0).IsName("Lifesteal")) { animator.SetFloat(animIDLifestealBackward, -1); }
        if (Player.setupState || cementary.groundCastUp || !Mouse.current.leftButton.isPressed) { animator.SetBool("Lifesteal", false); }
        else { Lifesteal(); }
    }

    private void Lifesteal()
    {
        animator.SetFloat(animIDLifestealBackward, 1);
        animator.SetBool("Lifesteal", true);

        if (radius < minRadius) { radius = minRadius; }
        
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 100f;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);

        Debug.DrawRay(attackPoint.position, mousePos - attackPoint.position, Color.blue);

        Vector3 direction = mousePos - attackPoint.position;
        direction.Normalize();

        RaycastHit hit;

        if (Physics.SphereCast(attackPoint.position, radius, direction, out hit, maxDistance, layer, QueryTriggerInteraction.UseGlobal))
        {
            cd = Time.time + 0.5f;
            float distanceFromPlayer = Vector3.Distance(hit.point, attackPoint.position);
            float t = Mathf.InverseLerp(0f, maxDistance, distanceFromPlayer);
            radius = Mathf.Lerp(minRadius, maxRadius, t);

            GameObject hitGO = hit.transform.gameObject;
            if (!EnemyList.enemies.Contains(hitGO)) { return; }

            hitGO.TryGetComponent(out Enemy Enemy);
            if (Time.time < nextFire) { return; }

            nextFire = Time.time + 1f;
            Enemy.TakeDamage(damage);
            Player.TakeHeal(heal);
        }
    }
}