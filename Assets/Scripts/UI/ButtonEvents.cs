using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    [SerializeField]
    private string _EnterSound = "enter_button";
    [SerializeField]
    private string _PressSound = "press_button";

    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundPlayer.Instance.Play(_EnterSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundPlayer.Instance.Play(_EnterSound);
    }
}
