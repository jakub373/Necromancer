using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveManagerZazura : MonoBehaviour
{
    private MainMenuSaves main;
    [SerializeField] private StatusBlood status;
    [SerializeField] private HealthBar bar;
    [SerializeField] private BloodBar bBar;

    private void Awake()
    {
        main = GetComponent<MainMenuSaves>();
        if (ES3.FileExists(main.fileRef))
        {
            status.health = ES3.Load("health", main.fileRef, status.health);
            bar.SetHealth(status.health);
            status.blood = ES3.Load("blood", main.fileRef, status.blood);
            bBar.SetBlood(status.blood);
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
        ES3.Save("blood", status.blood, text);
    }
}
