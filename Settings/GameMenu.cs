using UnityEngine;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    private void Update()
    {
        if (this.gameObject.activeSelf)
        {
            //Pause
        }
    }
    public void Return()
    {
        //Unpause
    }
    public void Exit()
    {
        //Save game and load Main Menu
        SceneManager.LoadScene("_MainMenu");
    }
}
