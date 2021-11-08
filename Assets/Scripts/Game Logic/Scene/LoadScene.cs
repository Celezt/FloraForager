using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using MyBox;

public class LoadScene : Singleton<LoadScene>
{
    [SerializeField]
    private Slider _ProgressSlider;

    public static event System.Action OnSceneBeingLoaded = delegate { };
    public static string ObjectToLoadPlayer = string.Empty;

    private CanvasGroup _CanvasGroup;

    private bool _SceneIsLoading = false;

    private void Awake()
    {
        _CanvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        FadeScreen.Instance.StartFadeOut(1f);
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

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        _ProgressSlider.value = 0.0f;
        yield return new WaitForSeconds(0.25f);

        OnSceneBeingLoaded.Invoke();

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);

        while (!asyncLoad.isDone)
        {
            _ProgressSlider.value = asyncLoad.progress / 0.9f;
            yield return null;
        }

        _SceneIsLoading = false;
    }

    private void SceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!string.IsNullOrWhiteSpace(ObjectToLoadPlayer))
        {
            GameObject player = PlayerInput.GetPlayerByIndex(0).gameObject;
            PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
            Bounds playerBounds = player.GetComponent<Collider>().bounds;

            LoadSceneTrigger[] triggers = FindObjectsOfType<LoadSceneTrigger>();
            foreach (LoadSceneTrigger trigger in triggers)
            {
                if (trigger.ObjectID == ObjectToLoadPlayer)
                {
                    player.transform.position = trigger.PlayerPosition + Vector3.up * playerBounds.extents.y;
                    playerMovement.SetDirection((trigger.PlayerRotation * Vector3.forward).xz()); // TODO: does not work, fix somehow

                    break;
                }
            }
            ObjectToLoadPlayer = string.Empty;
        }
    }
}
