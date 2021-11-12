using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class ConfirmMenuFactory : Singleton<ConfirmMenuFactory>
{
    [SerializeField]
    private GameObject _ConfirmMenuPrefab;

    public GameObject CreateMenu(Transform parent, string warningText, UnityAction yesAction, UnityAction noAction)
    {
        GameObject gameObject = Instantiate(_ConfirmMenuPrefab, parent);
        gameObject.GetComponent<ConfirmMenu>().SetMenu(warningText, yesAction, noAction);
        return gameObject;
    }
    public GameObject CreateMenu(Transform parent, bool first, string warningText, UnityAction yesAction, UnityAction noAction)
    {
        GameObject gameObject = Instantiate(_ConfirmMenuPrefab, parent);
        gameObject.GetComponent<ConfirmMenu>().SetMenu(warningText, yesAction, noAction);

        if (first) 
            gameObject.transform.SetAsFirstSibling();

        return gameObject;
    }
    public GameObject CreateMenu(string warningText, UnityAction yesAction, UnityAction noAction)
    {
        GameObject gameObject = Instantiate(_ConfirmMenuPrefab, transform);
        gameObject.GetComponent<ConfirmMenu>().SetMenu(warningText, yesAction, noAction);
        return gameObject;
    }
}
