using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using MyBox;

public class GameTimeUI : Singleton<GameTimeUI>
{
    [SerializeField] 
    private TMP_Text _Date;
    [SerializeField] 
    private TMP_Text _Time;

    public void UpdateText(string date, string time)
    {
        _Date.text = date;
        _Time.text = time;
    }
}
