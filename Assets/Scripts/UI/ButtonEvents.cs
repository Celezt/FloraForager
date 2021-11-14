using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonEvents : MonoBehaviour, IPointerEnterHandler, IPointerDownHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        SoundPlayer.Instance.Play("hover_button");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        SoundPlayer.Instance.Play("hover_button");
    }
}
