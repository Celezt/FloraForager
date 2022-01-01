using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;
using MyBox;

public class MainMenu : MonoBehaviour
{
    [Space(5)]
    [SerializeField] 
    private Button _ContinueButton;

    [Space(10)]
    [SerializeField]
    private bool _LoadFirstScene;
    [SerializeField, Scene, ShowIf("_LoadFirstScene")]
    private string _FirstSceneName;

    private GameObject _ConfirmMenu;

    private string[] _SaveFiles;
    private const string SAVE_NAME = "save";

    private void Awake()
    {
        _SaveFiles = GameManager.GetSaves();
        _ContinueButton.interactable = GameManager.SaveExists(SAVE_NAME);

        Time.timeScale = 1.0f;
    }

    public void OnContinue()
    {
        GameManager.LoadGame();

        if (PlayerDataMaster.Instance.Exists(0) && PlayerDataMaster.Instance.Get(0).SaveData.SceneIndex != 0) // hard-coded for now
            LoadScene.Instance.LoadSceneByIndex(PlayerDataMaster.Instance.Get(0).SaveData.SceneIndex);
        else
        {
            if (_LoadFirstScene)
                LoadScene.Instance.LoadSceneByName(_FirstSceneName);
            else
                LoadScene.Instance.LoadSceneByIndex(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void OnStartNew()
    {
        if (GameManager.SaveExists(SAVE_NAME))
        {
            if (_ConfirmMenu != null)
                return;

            _ConfirmMenu = ConfirmMenuFactory.Instance.CreateMenu("This will delete your current save, are you sure?", 1,
                new UnityAction(() => 
                {
                    GameManager.DeleteSave(SAVE_NAME);
                    OnContinue();
                }), 
                new UnityAction(() => { }));
        }
        else
        {
            OnContinue();
        }
    }

    public void OnQuit()
    {
        ConsoleUtility.QuitGame();
    }

    public void OnSurvey()
    {
        //Application.OpenURL("https://forms.gle/hHU988g859QK2Gbx6");
        Application.OpenURL("https://forms.gle/xkPvQVAhkxpJd2Mm6");
    }
}
