using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuSaves : MonoBehaviour
{
    public List<GameObject> loadList, saveList = new List<GameObject>();
    public List<string> files = new List<string>();
    [SerializeField] private GameObject loadContent, saveContent, loadSlot, saveSlot;
    [SerializeField] private Button continueButton;

    public RuntimeAnimatorController controller;
    public TMP_FontAsset fontAsset;
    public Transform player;
    public TMP_InputField inputText;

    public static string file;
    [HideInInspector] public string fileRef;
    public int saveNum;
    public string newestFile = null, scene;

    private void Awake()
    {
        fileRef = file;
        if (ES3.FileExists(file))
        {
            saveNum = ES3.Load("saveNum", file, saveNum);
            player.rotation = ES3.Load("heroRot", file, player.rotation);
            player.position = ES3.Load("heroPos", file, player.position);
        }
    }

    void Start()
    {
        var newestDateTime = new DateTime(0);

        foreach (var filename in ES3.GetFiles())
        {
            files.Add(filename);

            var thisDateTime = ES3.GetTimestamp(filename);
            if (thisDateTime > newestDateTime)
            {
                newestFile = filename;
                newestDateTime = thisDateTime;
            }
        }

        if (ES3.FileExists(newestFile)) { saveNum = ES3.Load("saveNum", newestFile, saveNum); }
        else { if (continueButton != null) continueButton.enabled = false; }
    }

    public void SetSaves(bool loadGame)
    {
        List<string> filesStatic = new List<string>();
        filesStatic.Clear();
        files.Clear();
        foreach(GameObject go in saveList) { Destroy(go); }
        foreach(GameObject go in loadList) { Destroy(go); }
        saveList.Clear();
        loadList.Clear();

        var newestDateTime = new DateTime(0);

        foreach (var filename in ES3.GetFiles())
        {
            files.Add(filename);
            filesStatic.Add(filename);

            var thisDateTime = ES3.GetTimestamp(filename);
            if (thisDateTime > newestDateTime)
            {
                newestFile = filename;
                newestDateTime = thisDateTime;
            }
        }

        if (ES3.FileExists(newestFile))
        {
            saveNum = ES3.Load("saveNum", newestFile, saveNum);

            //Sort files based on DateTime.
            foreach (var file in filesStatic)
            {
                List<string> temp = new List<string>();
                temp.Clear();
                temp = new List<string>(files);

                for (int i = 0; i <= files.Count - 1; i++)
                {
                    if (ES3.GetTimestamp(file) > ES3.GetTimestamp(temp[i]))
                    {
                        var item = temp[i];
                        var item2 = files.IndexOf(file);
                        var index = files.IndexOf(item);
                        files[index] = file;
                        files.RemoveAt(item2);
                        files.Add(item);
                    }
                }
            }
            if (loadGame) AddNewSaveObject(loadList, loadSlot, loadContent);
            else AddNewSaveObject(saveList, saveSlot, saveContent);
        }
    }
    public void AddNewSaveObject(List<GameObject> gObjList, GameObject obj, GameObject content)
    {
        //Instantiate new bar gameobject.
        while (gObjList.Count != files.Count)
        {
            GameObject newSave = Instantiate(obj);
            newSave.transform.SetParent(content.transform);
            gObjList.Add(newSave);
        }

        //Set name for game objects.
        foreach (GameObject save in gObjList)
        {
            if (save.TryGetComponent(out LoadMenu loadScript))
            {
                int i = gObjList.IndexOf(save);
                loadScript.text.text = files[i] + " - " + ES3.GetTimestamp(files[i]);
                loadScript.button.onClick.AddListener(loadScript.Load);
            }
            if (save.TryGetComponent(out SaveMenu saveScript))
            {
                int i = gObjList.IndexOf(save);
                saveScript.text.text = files[i] + " - " + ES3.GetTimestamp(files[i]);
                saveScript.button.onClick.AddListener(saveScript.Save);
                saveScript.filename = files[i];
            }
        }
    }
    public void LoadGame(int fileIndex)
    {
        if (ES3.FileExists(files[fileIndex]))
        {
            scene = ES3.Load<string>("scene", files[fileIndex]);
            SceneManager.LoadScene(scene);
            file = files[fileIndex];
        }
    }
    public void Continue()
    {
        if (ES3.FileExists(newestFile))
        {
            scene = ES3.Load<string>("scene", newestFile);
            SceneManager.LoadScene(scene);
            file = newestFile;
        }
    }
    void Save(string text)
    {
        scene = SceneManager.GetActiveScene().name;
        ES3.Save("heroPos", player.position, text);
        ES3.Save("heroRot", player.rotation, text);
        ES3.Save("scene", scene, text);
        AddNewSaveObject(saveList, saveSlot, saveContent);
    }
    public void AutoSaveGame()
    {
        while (ES3.FileExists("SaveFile" + saveNum)) { saveNum++; }

        string text = "SaveFile" + saveNum;

        ES3.Save("saveNum", saveNum, text);
        Save(text);
    }
    public void SaveGame()
    {
        string text = inputText.text;
        Save(text);
    }

    public void SaveGameOnCurrentFile()
    {
        string text = EventSystem.current.currentSelectedGameObject.GetComponent<SaveMenu>().filename;

        ES3.Save("saveNum", saveNum, text);
        Save(text);
    }
}