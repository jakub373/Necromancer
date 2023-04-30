using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;

public class ReapersUrn_Shoot : InumarAbilitySystem_Shoot
{
    private WaitForEndOfFrame frame = new WaitForEndOfFrame();
    private Rigidbody rigid;
    [SerializeField] private Camera fpsCam;
    [SerializeField] private GameObject circle;
    [SerializeField] private Canvas AoESpell;
    [SerializeField] private LayerMask groundLayer;
    
    public bool setUrn = false, groundCastUp = false;
    [SerializeField] private float maxDistance;

    public override void Awake()
    {
        base.Awake();

        circle.SetActive(false);

        abilityImage = GameObject.Find("Skill1 BW").GetComponent<Image>();
        abilityImage.fillAmount = 0;

        rigid = prefab.GetComponentInChildren<Rigidbody>();
    }
    private void Update()
    {
        rigid.transform.rotation = Quaternion.identity;
        GlobalCooldownCheck();
        AbilityCooldown();
        if (status.setupState == true) { return; }
        AOETarget(); 
        Urn();
    }

    void Urn()
    {
        if (cementary.groundCastUp) { return; }
        if (setUrn) { ChangeRope(); } else { ReturnRope(); }

        if (Mouse.current.rightButton.isPressed && !cementary.groundCastUp && !setUrn && !groundCastUp && !isCooldown && !status.globalCooldown())
        {
            groundCastUp = true;
            circle.SetActive(true);
        }

        if (Mouse.current.rightButton.wasReleasedThisFrame && groundCastUp && !setUrn)
        {
            status.GlobalCooldown();
            abilityImage.fillAmount = 1;
            if (!fire.isCooldown) fire.abilityImage.fillAmount = 1;
            if (!cementary.isCooldown) cementary.abilityImage.fillAmount = 1;
            rigid.transform.position = AoESpell.transform.position + 0.5f * Vector3.up;
            rigid.transform.rotation = Quaternion.Euler(0,0,0);
            circle.SetActive(false); 
            groundCastUp = false;

            StartCoroutine("WaitForFrame");
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && !cementary.groundCastUp && setUrn && !status.globalCooldown()) 
        {
            Return();
        }
    }
    private IEnumerator WaitForFrame()
    {
        yield return frame;
        setUrn = true;
    }
    public void Return()
    {
        setUrn = false;
        isCooldown = true;
        abilityImage.fillAmount = 1;
    }
    private void ChangeRope()
    {
        rigid.constraints = RigidbodyConstraints.FreezeAll;
        rigid.transform.rotation = Quaternion.Euler(Vector3.zero);
        rigid.transform.SetParent(null);
    }

    private void ReturnRope ()
    {
        rigid.constraints = RigidbodyConstraints.None;
        rigid.constraints = RigidbodyConstraints.FreezeRotationX;
        rigid.constraints = RigidbodyConstraints.FreezeRotationY;
        rigid.transform.SetParent(prefab.transform);
        rigid.transform.position = prefab.transform.position - Vector3.up * 0.5f;
    }
    private void AOETarget()
    {
        if (!groundCastUp) { return; }

        //Ustalenie kierunku promienia w przestrzeñ ("niewidzialny" punkt)
        RaycastHit hit;
        float distance;
        Ray ray = fpsCam.ScreenPointToRay(Mouse.current.position.ReadValue());

        // Namierzanie na ziemi
        if (!Physics.Raycast(ray, out hit, Mathf.Infinity, groundLayer)) { return; }
        if (hit.collider.gameObject == this.gameObject) { return; }

        var hitPosDir = (hit.point - transform.position).normalized;
        distance = Vector3.Distance(hit.point, transform.position);
        distance = Mathf.Min(distance, maxDistance);

        var newHitPos = transform.position + hitPosDir * distance;
        AoESpell.transform.position = (newHitPos);
    }
}