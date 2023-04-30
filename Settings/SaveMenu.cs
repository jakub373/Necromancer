using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SaveMenu : MonoBehaviour
{
    [ES3Serializable] private Animator animator;

    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] public Button button;

    private MainMenuSaves save;
    private SaveManagerInumar inu;
    private SaveManagerSarin sar;
    private SaveManagerLesarin les;
    private SaveManagerRael rael;
    private SaveManagerZazura zaz;
    public string filename;

    private void Awake()
    {
        save = GameObject.Find("Menu").GetComponent<MainMenuSaves>();
        inu = GameObject.Find("Menu").GetComponent<SaveManagerInumar>();
        sar = GameObject.Find("Menu").GetComponent<SaveManagerSarin>();
        les = GameObject.Find("Menu").GetComponent<SaveManagerLesarin>();
        rael = GameObject.Find("Menu").GetComponent<SaveManagerRael>();
        zaz = GameObject.Find("Menu").GetComponent<SaveManagerZazura>();
        button = GetComponent<Button>();

        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = save.controller as RuntimeAnimatorController;

        text = GetComponentInChildren<TextMeshProUGUI>();
        text.font = save.fontAsset;
    }

    public void Save()
    {
        save.SaveGameOnCurrentFile();
        if (inu != null) inu.SaveGameOnCurrentFile();
        if (sar != null) sar.SaveGameOnCurrentFile();
        if (les != null) les.SaveGameOnCurrentFile();
        if (rael != null) rael.SaveGameOnCurrentFile();
        if (zaz != null) zaz.SaveGameOnCurrentFile();
    }
}
