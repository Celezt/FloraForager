using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TutorialMenu : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    private GameObject _Tutorial;
    [SerializeField]
    private Image _Background;
    [SerializeField]
    private Color _TintColor;

    private Color _NormalColor;

    private void Awake()
    {
        _Tutorial.SetActive(false);
        _NormalColor = _Background.color;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _Tutorial.SetActive(true);
        _Background.color = _NormalColor * _TintColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _Tutorial.SetActive(false);
        _Background.color = _NormalColor;
    }
}
