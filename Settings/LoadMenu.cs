using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadMenu : MonoBehaviour
{
    [ES3Serializable] private Animator animator;

    [SerializeField] public TextMeshProUGUI text;
    [SerializeField] public Button button;

    private MainMenuSaves save;

    private void Awake()
    {
        save = GameObject.Find("Menu").GetComponent<MainMenuSaves>();
        button = GetComponent<Button>();

        animator = GetComponent<Animator>();
        animator.runtimeAnimatorController = save.controller as RuntimeAnimatorController;

        text = GetComponentInChildren<TextMeshProUGUI>();
        text.font = save.fontAsset;
    }

    public void Load()
    {
        save.LoadGame(save.loadList.IndexOf(this.gameObject));
    }
}