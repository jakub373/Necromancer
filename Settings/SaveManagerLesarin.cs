using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveManagerLesarin : MonoBehaviour
{
    private MainMenuSaves main;
    [SerializeField] private StatusNature status;
    [SerializeField] private HealthBar bar;
    [SerializeField] private SpiritualPowerBar sBar;

    private void Awake()
    {
        main = GetComponent<MainMenuSaves>();
        if (ES3.FileExists(main.fileRef))
        {
            status.health = ES3.Load("health", main.fileRef, status.health);
            bar.SetHealth(status.health);
            status.spirit = ES3.Load("spirit", main.fileRef, status.spirit);
            sBar.SetSpirit(status.spirit);
            status.seed = ES3.Load("seed", main.fileRef, status.seed);
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
        ES3.Save("spirit", status.spirit, text);
        ES3.Save("seed", status.seed, text);
    }
}
