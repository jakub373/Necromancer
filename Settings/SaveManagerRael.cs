using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SaveManagerRael : MonoBehaviour
{
    private MainMenuSaves main;
    [SerializeField] private StatusFrost status;
    [SerializeField] private HealthBar bar;
    [SerializeField] private ManaBar mBar;

    private void Awake()
    {
        main = GetComponent<MainMenuSaves>();
        if (ES3.FileExists(main.fileRef))
        {
            status.health = ES3.Load("health", main.fileRef, status.health);
            bar.SetHealth(status.health);
            status.mana = ES3.Load("mana", main.fileRef, status.mana);
            mBar.SetMana(status.mana);
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
        ES3.Save("mana", status.mana, text);
    }
}
