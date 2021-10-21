using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class LoadSceneTrigger : MonoBehaviour
{
    [SerializeField]
    private string _ObjectID = string.Empty; // only needs to be unique in this scene to act as an identifier for loading position & rotation
    [SerializeField]
    private string _SceneToLoad;

    [Header("Next Scene Load")]
    [SerializeField]
    private string _ObjectIDToLoadPlayer; // id of the object in the newly loaded scene to assign player position and rotation
    [SerializeField]
    private Vector3 _PlayerLoadPosition;
    [SerializeField]
    private Quaternion _PlayerLoadRotation;

    private GameObject _Player;
    private BoxCollider _Collider;

    private bool _NoCollision;

    public string ObjectID => _ObjectID;

    public Vector3 PlayerPosition => transform.position + _PlayerLoadPosition;
    public Quaternion PlayerRotation => _PlayerLoadRotation;

    private void Awake()
    {
        _Collider = GetComponent<BoxCollider>();
        _Player = PlayerInput.GetPlayerByIndex(0).gameObject;

        _NoCollision = true;
        StartCoroutine(CheckPlayerCollision());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (FadeScreen.Instance.IsActive || _NoCollision || other.gameObject != _Player)
            return;

        FadeScreen.Instance.StartFadeIn(1f);
        FadeScreen.Instance.OnEndFade += LoadLevel;

        _NoCollision = true;
    }

    public void LoadLevel()
    {
        LoadScene.ObjectToLoadPlayer = _ObjectIDToLoadPlayer;
        LoadScene.Instance.LoadSceneByName(_SceneToLoad);

        FadeScreen.Instance.OnEndFade -= LoadLevel;
    }

    private IEnumerator CheckPlayerCollision()
    {
        yield return new WaitForSeconds(0.1f);

        while (true)
        {
            _NoCollision = false;

            Collider[] colliders = Physics.OverlapBox(transform.position, _Collider.size / 2.0f, transform.rotation);
            foreach (Collider collider in colliders)
            {
                if (collider.gameObject == _Player)
                {
                    _NoCollision = true;
                    break;
                }
            }

            if (!_NoCollision)
                break;

            yield return new WaitForSeconds(0.5f);
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.DrawWireDisc(transform.position + _PlayerLoadPosition, Vector3.up, 1.0f, 2.0f);
        Gizmos.DrawRay(transform.position + _PlayerLoadPosition, _PlayerLoadRotation * Vector3.forward);
    }
#endif
}
