using UnityEngine;
using System.Collections.Generic;

public class SetupPet : MonoBehaviour
{
    [SerializeField] private bool acceptable;
    private StatusShadow spawn;

    public List<GameObject> empty, melee, tank, range, pet = new List<GameObject>();
    public GameObject Empty1, Empty2, Empty3, Tank1, Tank2, Tank3, Melee1, Melee2, Melee3, Range1, Range2, Range3, accept, disabledAccept;

    private float cooldown = 0;
    private void Awake()
    {
        empty = new List<GameObject> { Empty1, Empty2, Empty3 };
        melee = new List<GameObject> { Melee1, Melee2, Melee3 };
        tank = new List<GameObject> { Tank1, Tank2, Tank3 };
        range = new List<GameObject> { Range1, Range2, Range3 };

        pet = new List<GameObject> { Tank1, Melee1, Range1, Tank2, Melee2, Range2, Tank3, Melee3, Range3 };

        foreach (GameObject go in empty) { go.SetActive(true); }
        foreach (GameObject go in pet) { go.SetActive(false); }

        acceptable = false;
        accept.SetActive(false);
        disabledAccept.SetActive(true);

        spawn = GameObject.Find("Inumar").GetComponent<StatusShadow>();
    }

    private void Update() => CheckAcceptButton();

    //Sprawdü czy pet picki sπ wybrane
    private void CheckAcceptButton()
    {
        if (!spawn.setupState) { return; }
        if (Empty1.activeSelf || Empty2.activeSelf || Empty3.activeSelf) { acceptable = false; }
        else { acceptable = true; }

        if (acceptable) { accept.SetActive(true); disabledAccept.SetActive(false); }
        else { accept.SetActive(false); disabledAccept.SetActive(true); }
    }

    private void Pick(List<GameObject> list)
    {
        for (int i = 0; i < 3; i++)
        {
            if (empty[i].activeSelf)
            {
                empty[i].SetActive(false);
                list[i].SetActive(true);
                return;
            }
        }
    }
    public void PickTank() => Pick(tank);
    public void PickMelee() => Pick(melee);
    public void PickRange() => Pick(range);

    public void RemovePick(int i)
    {
        if (!empty[i].activeSelf)
        {
            empty[i].SetActive(true);
            tank[i].SetActive(false);
            melee[i].SetActive(false);
            range[i].SetActive(false);
            return;
        }
    }
    public void RemovePick1() => RemovePick(0);
    public void RemovePick2() => RemovePick(1);
    public void RemovePick3() => RemovePick(2);

    public void SpawnPet(GameObject go)
    {
        if (melee.Contains(go)) { spawn.SpawnMelee(); }
        if (range.Contains(go)) { spawn.SpawnRange(); }
        if (tank.Contains(go)) { spawn.SpawnTank(); }
    }
    public void SwitchPet(GameObject go)
    {
        if (melee.Contains(go)) { spawn.SwitchMelee(); }
        if (range.Contains(go)) { spawn.SwitchRange(); }
        if (tank.Contains(go)) { spawn.SwitchTank(); }
    }
    public void Spawn()
    {
        if (!acceptable) return;
        if (Time.time <= cooldown) return; 

        if (spawn.pets.Count == 3)
        {
            for (int x = 8; x > -1; x--)
            {
                var go = this.pet[x];
                if (go.activeSelf)
                {

                    var pet = spawn.petStatus;
                    if (range.Contains(go))
                    {
                        int i = range.IndexOf(go);
                        if (pet[i].name != "Range" && pet[i].name != "Range(Clone)")
                        {
                            pet[i].SwitchDie();
                            SwitchPet(go);
                        }
                    }
                    if (melee.Contains(go))
                    {
                        int i = melee.IndexOf(go);
                        if (pet[i].name != "Melee" && pet[i].name != "Melee(Clone)")
                        {
                            pet[i].SwitchDie();
                            SwitchPet(go);
                        }
                    }
                    if (tank.Contains(go))
                    {
                        int i = tank.IndexOf(go);
                        if (pet[i].name != "Tank" && pet[i].name != "Tank(Clone)")
                        {
                            pet[i].SwitchDie();
                            SwitchPet(go);
                        }
                    }
                }
            }
        }
        else
        {
            for (int x = 0; x < 9; x++)
            {
                var go = this.pet[x];
                if (go.activeSelf)
                {
                    var pet = spawn.petStatus;
                    if (tank.Contains(go))
                    {
                        int i = tank.IndexOf(go);
                        if (pet[i] != null)
                        {
                            if (pet[i].name != "Tank" && pet[i].name != "Tank(Clone)")
                            {
                                pet[i].SwitchDie();
                                SwitchPet(go);
                            }
                        }
                        else { SpawnPet(go); }

                    }
                    if (melee.Contains(go))
                    {
                        int i = melee.IndexOf(go);
                        if (pet[i] != null)
                        {
                            if (pet[i].name != "Melee" && pet[i].name != "Melee(Clone)")
                            {
                                pet[i].SwitchDie();
                                SwitchPet(go);
                            }
                        }
                        else { SpawnPet(go); }

                    }
                    if (range.Contains(go))
                    {
                        int i = range.IndexOf(go);
                        if (pet[i] != null)
                        {
                            if (pet[i].name != "Range" && pet[i].name != "Range(Clone)")
                            {
                                pet[i].SwitchDie();
                                SwitchPet(go);
                            }
                        }
                        else { SpawnPet(go); }
                    }
                }
            }
        }
        foreach (GameObject go in empty) { go.SetActive(true); }

        spawn.setupState = false;
        cooldown = Time.time + 1f;
    }

}