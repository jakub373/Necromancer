using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
public class CementaryGround_Shoot : InumarAbilitySystem_Shoot
{
    //[SerializeField] private CinemachineVirtualCamera followCamera;
    //[SerializeField] private CinemachineVirtualCamera AOECamera;
    [SerializeField] private Animator animator;

    public GameObject prefabPS;
    private GameObject circle;
    private Canvas AoESpell;
    private Camera fpsCam;
    private CementaryGround ground;
    private WaitForEndOfFrame frame = new WaitForEndOfFrame();

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float maxDistance, heal;
    public bool groundCastUp = false;
    [HideInInspector] public bool cementaryBool = false;
    private int animIDCementaryBackward;

    public override void Awake()
    {
        base.Awake();

        GameObject cameraGObj = GameObject.Find("ThirdPersonCamera");
        fpsCam = cameraGObj.GetComponent<Camera>();

        GameObject canvas = GameObject.Find("AoE");
        AoESpell = canvas.GetComponent<Canvas>();

        circle = GameObject.Find("Cementary");
        circle.SetActive(false);

        abilityImage = GameObject.Find("Skill3 BW").GetComponent<Image>();
        abilityImage.fillAmount = 0;

        prefab = GameObject.Find("CementaryGround");
        ground = prefab.GetComponent<CementaryGround>();
        prefab.SetActive(false);

        animIDCementaryBackward = Animator.StringToHash("CementaryBackward");
    }
    
    private void Update()
    {
        GlobalCooldownCheck();
        AbilityCooldown();
        if (status.setupState == true) { return; }
        AOETarget();
        Cementary();
    }

    private void Cementary()
    {
        if (Mouse.current.leftButton.wasReleasedThisFrame && animator.GetCurrentAnimatorStateInfo(0).IsName("CementaryStart")) { animator.SetFloat(animIDCementaryBackward, -1); }

        if (urn.groundCastUp) { return; }
        //if (groundCastUp) { CameraSwitcher.SwitchCamera(AOECamera); Cursor.lockState = CursorLockMode.Confined; } 
        //else { CameraSwitcher.SwitchCamera(followCamera); Cursor.lockState = CursorLockMode.Locked; }

        if (!groundCastUp && (Mouse.current.leftButton.isPressed || Mouse.current.rightButton.isPressed)) { return; }

        if (groundCastUp && (Mouse.current.rightButton.wasReleasedThisFrame || Mouse.current.leftButton.wasReleasedThisFrame || Keyboard.current.qKey.wasReleasedThisFrame 
            || Keyboard.current.rKey.wasReleasedThisFrame || Keyboard.current.tabKey.wasReleasedThisFrame || Keyboard.current.escapeKey.wasReleasedThisFrame))
        {
            groundCastUp = false;
            circle.SetActive(false);
            return;
        }

        if (!groundCastUp && Keyboard.current.eKey.wasPressedThisFrame && !isCooldown && !status.globalCooldown())
        {
            circle.SetActive(true);
            StartCoroutine("WaitForFrame");
        }

        if (groundCastUp && Keyboard.current.eKey.wasPressedThisFrame)
        {
            prefab.SetActive(true);
            prefab.transform.position = AoESpell.transform.position;
            
            circle.SetActive(false);
            groundCastUp = false;
            cementaryBool = true;
            animator.SetFloat(animIDCementaryBackward, 1);
            animator.SetBool("Cementary", true);
        }

        if (cementaryBool && Keyboard.current.eKey.wasReleasedThisFrame)
        {
            cementaryBool = false;
            prefab.SetActive(false);
            prefab.transform.position = this.gameObject.transform.position;

            status.GlobalCooldown();
            if(!fire.isCooldown) fire.abilityImage.fillAmount = 1;
            if(!urn.isCooldown) urn.abilityImage.fillAmount = 1;

            isCooldown = true;
            abilityImage.fillAmount = 1;

            status.TakeHeal(ground.stack * heal);
            ground.stack = 0;

            animator.SetBool("Cementary", false);
        }
    }

    private IEnumerator WaitForFrame()
    {
        yield return frame;
        groundCastUp = true;
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