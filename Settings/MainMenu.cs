using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("_NewGame");
    }
    public void ExitButton()
    {
        Application.Quit();
    }
}