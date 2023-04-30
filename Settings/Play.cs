using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Play : MonoBehaviour
{
    public void RaelPick()
    {
        SceneManager.LoadScene("Rael_1");
    }
    public void LesarinPick()
    {
        SceneManager.LoadScene("Lesarin_1");
    }
    public void SarinPick()
    {
        SceneManager.LoadScene("Sarin_1");
    }
    public void InumarPick()
    {
        SceneManager.LoadScene("Inumar_1");
    }
    public void ZazuraPick()
    {
        SceneManager.LoadScene("Zazura_1");
    }
    public void Back()
    {
        SceneManager.LoadScene("_MainMenu");
    }
}
