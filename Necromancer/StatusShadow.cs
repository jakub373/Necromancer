using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Cinemachine;

public class StatusShadow : MonoBehaviour, IDamageable
{
    [SerializeField] private CinemachineVirtualCamera followCamera, AOECamera;

    public List<GameObject> pets = new List<GameObject>();
    public List<Pet> petStatus = new List<Pet>();
    public List<HealthBarPet> petBars = new List<HealthBarPet>();
    private List<Image> icons = new List<Image>();

    [SerializeField] private Pet statusPet1, statusPet2, statusPet3;
    [SerializeField] private Pet meleePrefab, rangedPrefab, tankPrefab;
    private HealthBarPet healthBarPet1, healthBarPet2, healthBarPet3;

    private ObjectPool MeleePool, TankPool, RangedPool;

    public GameObject setupObj, crosshair, gameMenu, mainMenu, settings, exit, load, save;
    public Texture2D cursorArrow;
    private HealthBar healthBar;
    private SetupPet setup;
    [SerializeField] private Image icon1, icon2, icon3;

    public bool setupState = false;
    public float health, maxHealth, regenTime, timeBetweenRegenTick, healthRegen, gcd;

    private float gcdTime = 1.5f;

    [SerializeField] private Animator animator;

    private InumarController control;
    private Lifesteal_Shoot life;
    private ReapersFire_Shoot fire;
    private ReapersUrn_Shoot urn;
    private CementaryGround_Shoot cem;

    private int animIDDead;

    public bool globalCooldown()
    {
        if (gcd > Time.time) { return true; }
        else { return false; }
    }
    public void GlobalCooldown() => gcd = Time.time + gcdTime;

    private void OnEnable()
    {
        CameraSwitcher.Register(AOECamera);
        CameraSwitcher.Register(followCamera);
        CameraSwitcher.SwitchCamera(followCamera);
    }
    private void OnDisable()
    {
        CameraSwitcher.Unregister(AOECamera);
        CameraSwitcher.Unregister(followCamera);
    }
    public void Awake()
    {
        if (MeleePool == null) MeleePool = ObjectPool.CreateInstance(meleePrefab, 3);
        if (TankPool == null) TankPool = ObjectPool.CreateInstance(tankPrefab, 3);
        if (RangedPool == null) RangedPool = ObjectPool.CreateInstance(rangedPrefab, 3);


        setup = setupObj.GetComponent<SetupPet>();
        animIDDead = Animator.StringToHash("Dead");
        healthBar = GameObject.Find("HealthBar").GetComponent<HealthBar>();
        health = maxHealth;
        healthBar.SetMaxHealth(maxHealth);

        pets.AddRange(GameObject.FindGameObjectsWithTag("Pet"));

        healthBarPet1 = GameObject.Find("PetHealthBar(1)").GetComponent<HealthBarPet>();
        healthBarPet2 = GameObject.Find("PetHealthBar(2)").GetComponent<HealthBarPet>();
        healthBarPet3 = GameObject.Find("PetHealthBar(3)").GetComponent<HealthBarPet>();

        control = GetComponent<InumarController>();
        life = GetComponent<Lifesteal_Shoot>();
        fire = GetComponent<ReapersFire_Shoot>();
        urn = GetComponent<ReapersUrn_Shoot>();
        cem = GetComponent<CementaryGround_Shoot>();

        petBars = new List<HealthBarPet>() { healthBarPet1, healthBarPet2, healthBarPet3 };
        icons = new List<Image> { icon1, icon2, icon3 };
        petStatus = new List<Pet>() { statusPet1, statusPet2, statusPet3 };

        CheckDeadPets();
    }
    private void Update()
    {
        TestDamageHeal();
        UISetups();
        Menu();

        if (setupState || gameMenu.activeSelf)
        {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.SetCursor(cursorArrow, Vector2.zero, CursorMode.Auto);
            crosshair.SetActive(false);
        }
        else {
            Cursor.lockState = CursorLockMode.Locked;
            crosshair.SetActive(true);
        }

        if (setupState) { setupObj.SetActive(true); }
        else { setupObj.SetActive(false); }

        if (gameMenu.activeSelf) { gameMenu.SetActive(true); setupState = false; }
        else { gameMenu.SetActive(false); }

        if (setupState || gameMenu.activeSelf) return;
        if (cem.groundCastUp || urn.groundCastUp) { CameraSwitcher.SwitchCamera(AOECamera); Cursor.lockState = CursorLockMode.Confined; }
        else { CameraSwitcher.SwitchCamera(followCamera); Cursor.lockState = CursorLockMode.Locked; }
    }
    public void AddAlivePetsToSetup()
    {
        for (int i = 0; i < 3; i++)
        {
            var tank = setup.tank; var melee = setup.melee; var range = setup.range; var empty = setup.empty;
            if (petStatus[i] != null)
            {
                if (petStatus[i].name == "Tank" || petStatus[i].name == "Tank(Clone)")
                {
                    tank[i].SetActive(true);
                    melee[i].SetActive(false); range[i].SetActive(false); empty[i].SetActive(false);
                }
                else if (petStatus[i].name == "Melee" || petStatus[i].name == "Melee(Clone)")
                {
                    melee[i].SetActive(true);
                    tank[i].SetActive(false); range[i].SetActive(false); empty[i].SetActive(false);
                }
                else if (petStatus[i].name == "Ranged" || petStatus[i].name == "Ranged(Clone)")
                {
                    range[i].SetActive(true);
                    tank[i].SetActive(false); melee[i].SetActive(false); empty[i].SetActive(false);
                }
            }
            else { empty[i].SetActive(true); tank[i].SetActive(false); melee[i].SetActive(false); range[i].SetActive(false); }
        }
    }
    public void CheckDeadPets()
    {
        foreach (Pet pet in petStatus)
        {
            if (pet != null && pet.health <= 0) { RemovePet(pet.gameObject); }
        }
        IconColour();
    }
    public void RemovePet(GameObject pet) { pets.Remove(pet); }
    public void RemovePetOnSwitch(GameObject pet) { pets[pets.IndexOf(pet)] = null; }
    public void AddPet(ObjectPool pool)
    {
        if (pets.Count >= 3) { return; }
        
        PoolableObject gObj = pool.GetObject();
        gObj.transform.position = new Vector3(Random.insideUnitCircle.x * 7.5f, 1f, Random.insideUnitCircle.y * 7.5f);
        
        pets.Add(gObj.gameObject);

        IconColour();
    }
    public void SwitchPet(ObjectPool pool)
    {
        PoolableObject gObj = pool.GetObject();
        gObj.transform.position = new Vector3(Random.insideUnitCircle.x * 7.5f, 1f, Random.insideUnitCircle.y * 7.5f);
        if (pets[0] == null) { pets[0] = gObj.gameObject; }
        if (pets[1] == null) { pets[1] = gObj.gameObject; }
        if (pets[2] == null) { pets[2] = gObj.gameObject; }
        IconColour();
    }
    public void TakeDamage(float amount)
    {
        health -= amount - (amount * 0.1f * pets.Count);

        if (health <= 0) { Die(); }
        healthBar.SetHealth(health);
        if (health > 0)
        {
            for (int i = 2; i > -1; i--)
            {
                var pet = petStatus[i];
                if (pet != null)
                {
                    if (pet.name == "Tank" || pet.name == "Tank(Clone)") { pet.TakeDamage(0.2f * amount); }
                    else if (pet.name == "Melee" || pet.name == "Melee(Clone)") { pet.TakeDamage(0.15f * amount); }
                    else if (pet.name == "Ranged" || pet.name == "Ranged(Clone)") { pet.TakeDamage(0.1f * amount); }
                }
            }
        }
    }
    public void TakeHeal(float amount)
    {
        float missingHealth;
        missingHealth = maxHealth - health;

        if (missingHealth >= amount)
        {
            health += amount;
            healthBar.SetHealth(health);
        }
        else
        {
            float overheal;
            health = maxHealth;
            overheal = amount - missingHealth;
            healthBar.SetHealth(health);

            for (int i = 2; i > -1; i--)
            {
                var pet = petStatus[i];
                if (pet != null)
                {
                    if (pet.name == "Tank" || pet.name == "Tank(Clone)") { pet.TakeHeal(0.33f * overheal); }
                    else if (pet.name == "Melee" || pet.name == "Melee(Clone)") { pet.TakeHeal(0.25f * overheal); }
                    else if (pet.name == "Ranged" || pet.name == "Ranged(Clone)") { pet.TakeHeal(0.2f * overheal); }
                    petBars[i].SetHealth(pet.health);
                }
            }
        }
    }
    void IconColour()
    {
        for (int i = 0; i < 3; i++)
        {
            if (i < pets.Count)
            {
                petStatus[i] = pets[i].GetComponent<Pet>();
                petBars[i].gameObject.SetActive(true);
                petBars[i].SetNewUIHealth(petStatus[i].maxHealth, petStatus[i].health);
            }
            else
            {
                petStatus[i] = null;
                petBars[i].gameObject.SetActive(false);
            }
            statusPet1 = petStatus[0];
            statusPet2 = petStatus[1];
            statusPet3 = petStatus[2];
        }

        foreach (Pet pet in petStatus)
        {
            int i = petStatus.IndexOf(pet);
            if (pet == null) return;

            if (pet.name == "Tank" || pet.name == "Tank(Clone)") { icons[i].color = Color.blue; }
            else if (pet.name == "Melee" || pet.name == "Melee(Clone)") { icons[i].color = Color.red; }
            else if (pet.name == "Ranged" || pet.name == "Ranged(Clone)") { icons[i].color = Color.green; }
        }
    }
    public void HealthUse(float amount)
    {
        if (health < amount) { return; }

        health -= amount;
        healthBar.SetHealth(health);
    }
    public void HealthRegenOverTime()
    {
        if (Time.time > regenTime)
        {
            regenTime = Time.time + timeBetweenRegenTick;
            TakeHeal(healthRegen);
        }
    }
    public void Die()
    {
        animator.SetBool(animIDDead, true);
        cem.enabled = false;
        urn.enabled = false;
        control.enabled = false;
        life.enabled = false;
        fire.enabled = false;

        Debug.Log("You are killed!");
    }
    private void TestDamageHeal()
    {
        if (Keyboard.current.tabKey.wasPressedThisFrame) TakeDamage(100);
        if (Keyboard.current.spaceKey.wasPressedThisFrame) TakeHeal(200);
    }
    public void SpawnTank() => AddPet(TankPool);
    public void SpawnMelee() => AddPet(MeleePool);
    public void SpawnRange() => AddPet(RangedPool);
    public void SwitchTank() => SwitchPet(TankPool);
    public void SwitchMelee() => SwitchPet(MeleePool);
    public void SwitchRange() => SwitchPet(RangedPool);
    private void UISetups()
    {
        float next = 0;
        if (Keyboard.current.lKey.wasPressedThisFrame && next < Time.time)
        {
            next = Time.time + 0.1f;
            if (!setupState) { setupState = true; }
            else { setupState = false; }
            AddAlivePetsToSetup();
        }
    }
    private void Menu()
    {
        float next = 0;
        if (Keyboard.current.escapeKey.wasPressedThisFrame && next < Time.time)
        {
            next = Time.time + 0.1f;
            if (!gameMenu.activeSelf) { gameMenu.SetActive(true); }
            else if(!mainMenu.activeSelf) 
            { 
                mainMenu.SetActive(true); 
                settings.SetActive(false);
                exit.SetActive(false);
                load.SetActive(false);
                save.SetActive(false);
            }
            else { gameMenu.SetActive(false); }
        }
    }
    public Transform GetTransform()
    {
        return transform;
    }
}