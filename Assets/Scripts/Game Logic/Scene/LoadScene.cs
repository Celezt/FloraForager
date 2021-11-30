using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MyBox;

public class LoadScene : Singleton<LoadScene>
{
    [SerializeField]
    private Slider _ProgressSlider;

    public static event System.Action OnSceneBeingLoaded = delegate { };
    public static string ObjectToLoadPlayer = string.Empty;

    private CanvasGroup _CanvasGroup;
    private bool _SceneIsLoading;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoaded;
    }

    public bool LoadSceneByName(string sceneName)
    {
        if (_SceneIsLoading)
            return false;

        _CanvasGroup.alpha = 1.0f;
        _SceneIsLoading = true;

        StartCoroutine(LoadSceneAsync(sceneName));

        return true;
    }
    public bool LoadSceneByIndex(int sceneIndex)
    {
        if (_SceneIsLoading)
            return false;

        _CanvasGroup.alpha = 1.0f;
        _SceneIsLoading = true;

        StartCoroutine(LoadSceneAsync(sceneIndex));

        return true;
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        OnSceneBeingLoaded.Invoke();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        _ProgressSlider.value = 0.0f;
        while (!asyncLoad.isDone)
        {
            _ProgressSlider.value = asyncLoad.progress / 0.9f;
            yield return null;
        }
    }
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        OnSceneBeingLoaded.Invoke();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Single);

        _ProgressSlider.value = 0.0f;
        while (!asyncLoad.isDone)
        {
            _ProgressSlider.value = asyncLoad.progress / 0.9f;
            yield return null;
        }
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrWhiteSpace(ObjectToLoadPlayer))
        {
            GameObject player = PlayerInput.GetPlayerByIndex(0).gameObject;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();

            LoadSceneTrigger[] triggers = FindObjectsOfType<LoadSceneTrigger>();
            foreach (LoadSceneTrigger trigger in triggers)
            {
                if (trigger.ObjectID == ObjectToLoadPlayer)
                {
                    player.transform.position = trigger.PlayerPosition;
                    playerMovement.SetDirection((trigger.PlayerRotation * Vector3.forward).xz());

                    break;
                }
            }
            ObjectToLoadPlayer = string.Empty;
        }
    }
}
