using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class GameTimeUI : MonoBehaviour
{
    [SerializeField] 
    private TMP_Text _Date;
    [SerializeField] 
    private TMP_Text _Time;

    private void Start()
    {
        SetText();
    }

    private void LateUpdate()
    {
        SetText();
    }

    private void SetText()
    {
        _Date.text = GameTime.Instance.Weekday;
        _Time.text = GameTime.Instance.DigitalTime;
    }
}
