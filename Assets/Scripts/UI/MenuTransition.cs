using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuTransition : MonoBehaviour
{
    [Space(3), Header("Menus")]
    [SerializeField] private GameObject _Main;
    [SerializeField] private GameObject _Settings;
    [SerializeField] private GameObject _Controls;
    [SerializeField] private GameObject _Credits;
    [SerializeField] private GameObject _Back;

    private GameObject _CurrentMenu;

    private void Awake()
    {
        _Back.SetActive(false);
    }

    public void OpenSettings()
    {
        _Main.SetActive(false);
        _Settings.SetActive(true);
        _Back.gameObject.SetActive(true);

        _CurrentMenu = _Settings;
    }
    public void OpenControls()
    {
        _Main.SetActive(false);
        _Controls.SetActive(true);
        _Back.gameObject.SetActive(true);

        _CurrentMenu = _Controls;
    }
    public void OpenCredits()
    {
        _Main.SetActive(false);
        _Credits.SetActive(true);
        _Back.gameObject.SetActive(true);

        _CurrentMenu = _Credits;
    }
    public void Back()
    {
        _Main.SetActive(true);
        _CurrentMenu.SetActive(false);
        _Back.gameObject.SetActive(false);
    }
}
