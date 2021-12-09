using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MyBox;

public class ConfirmMenuFactory : Singleton<ConfirmMenuFactory>
{
    [SerializeField]
    private GameObject _ConfirmMenuPrefab;

    public GameObject CreateMenu(Transform parent, int sortOrder, string warningText, UnityAction yesAction, UnityAction noAction)
    {
        GameObject gameObject = Instantiate(_ConfirmMenuPrefab, parent);
        gameObject.GetComponent<ConfirmMenu>().SetMenu(warningText, yesAction, noAction);
        gameObject.GetComponent<Canvas>().sortingOrder = sortOrder;
        return gameObject;
    }
    public GameObject CreateMenu(Transform parent, int sortOrder, bool first, string warningText, UnityAction yesAction, UnityAction noAction)
    {
        GameObject gameObject = Instantiate(_ConfirmMenuPrefab, parent);
        gameObject.GetComponent<ConfirmMenu>().SetMenu(warningText, yesAction, noAction);
        gameObject.GetComponent<Canvas>().sortingOrder = sortOrder;

        if (first) 
            gameObject.transform.SetAsFirstSibling();

        return gameObject;
    }
    public GameObject CreateMenu(string warningText, int sortOrder, UnityAction yesAction, UnityAction noAction)
    {
        GameObject gameObject = Instantiate(_ConfirmMenuPrefab, transform);
        gameObject.GetComponent<ConfirmMenu>().SetMenu(warningText, yesAction, noAction);
        gameObject.GetComponent<Canvas>().sortingOrder = sortOrder;
        return gameObject;
    }
}
