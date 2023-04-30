using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static System.Net.Mime.MediaTypeNames;

public class SaveManagerInumar : MonoBehaviour
{
    private MainMenuSaves main;
    [SerializeField] private StatusShadow status;
    [SerializeField] private HealthBar bar;
    private string pet1, pet2, pet3;
    private Transform petPos1, petPos2, petPos3;
    private float petHealth1, petHealth2, petHealth3;

    private void Awake()
    {
        main = GetComponent<MainMenuSaves>();
        if (ES3.FileExists(main.fileRef))
        {
            status.health = ES3.Load("health", main.fileRef, status.health);
            bar.SetHealth(status.health);

            pet1 = ES3.Load("pet1", main.fileRef, "");
            pet2 = ES3.Load("pet2", main.fileRef, "");
            pet3 = ES3.Load("pet3", main.fileRef, "");
            List<string> pets = new List<string> { pet1, pet2, pet3 };

            foreach (string pet in pets)
            {
                if (pet == "Tank" || pet == "Tank(Clone)")
                {
                    status.SpawnTank();
                }
                else if (pet == "Melee" || pet == "Melee(Clone)")
                {
                    status.SpawnMelee();
                }
                else if (pet == "Ranged" || pet == "Ranged(Clone)")
                {
                    status.SpawnRange();
                }
            }
            var stat = status.petStatus;
            if (stat[0].gameObject != null)
            {
                stat[0].transform.position = ES3.Load("petPos1", main.fileRef, Vector3.zero);
                stat[0].health = ES3.Load("petHealth1", main.fileRef, stat[0].health);
                status.petBars[0].SetHealth(stat[0].health); ;
            }
            if (stat[1].gameObject != null)
            {
                stat[1].transform.position = ES3.Load("petPos2", main.fileRef, Vector3.zero);
                stat[1].health = ES3.Load("petHealth2", main.fileRef, stat[1].health);
                status.petBars[1].SetHealth(stat[1].health);
            }
            if (stat[2].gameObject != null)
            {
                stat[2].transform.position = ES3.Load("petPos3", main.fileRef, Vector3.zero);
                stat[2].health = ES3.Load("petHealth3", main.fileRef, stat[2].health);
                status.petBars[2].SetHealth(stat[2].health);
            }
        }
        this.gameObject.SetActive(false);
    }

    public void AutoSaveGame()
    {
        string text = "SaveFile" + main.saveNum;
        Save(text);
    }
    public void SaveGame()
    {
        string text = main.inputText.text;

        Save(text);
    }

    public void SaveGameOnCurrentFile()
    {
        string text = EventSystem.current.currentSelectedGameObject.GetComponent<SaveMenu>().filename;

        Save(text);
    }

    void Save(string text)
    {
        ES3.Save("health", status.health, text);

        var pets = status.pets;
        var stat = status.petStatus;
        if (stat[0] != null) { pet1 = pets[0].name; petPos1 = pets[0].transform; petHealth1 = stat[0].health; }
        else pet1 = "";
        if (stat[1] != null) { pet2 = pets[1].name; petPos2 = pets[1].transform; petHealth2 = stat[1].health; }
        else pet2 = "";
        if (stat[2] != null) { pet3 = pets[2].name; petPos3 = pets[2].transform; petHealth3 = stat[2].health; }
        else pet3 = "";
        ES3.Save("pet1", pet1, text);
        ES3.Save("pet2", pet2, text);
        ES3.Save("pet3", pet3, text);
        ES3.Save("petPos1", petPos1.position, text);
        ES3.Save("petPos2", petPos2.position, text);
        ES3.Save("petPos3", petPos3.position, text);
        ES3.Save("petHealth1", petHealth1, text);
        ES3.Save("petHealth2", petHealth2, text);
        ES3.Save("petHealth3", petHealth3, text);
    }
}