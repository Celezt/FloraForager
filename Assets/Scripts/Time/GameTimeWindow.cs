using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MyBox;

public class GameTimeWindow : Singleton<GameTimeWindow>
{
    [SerializeField] private Text _Date;
    [SerializeField] private Text _Time;

    public void UpdateText(string date, string time)
    {
        _Date.text = date;
        _Time.text = time;
    }
}
