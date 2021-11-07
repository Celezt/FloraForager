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

    public void LateUpdate()
    {
        _Date.text = GameTime.Instance.Weekday;
        _Time.text = GameTime.Instance.DigitalTime;
    }
}
