using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommissionObject : MonoBehaviour
{
    private Text _Text;

    private bool _MarkedCompleted = false;

    public Commission Commission { get; set; }

    private void Awake()
    {
        _Text = GetComponent<Text>();
    }

    void Update()
    {
        
    }

    public void Select()
    {
        _Text.color = Color.red;
        CommissionLog.Instance.ShowDescription(Commission);
    }

    public void Deselect()
    {
        _Text.color = Color.gray;
    }

    public void IsCompleted()
    {
        if (Commission.IsCompleted && !_MarkedCompleted)
        {
            _MarkedCompleted = true;
            _Text.text += " (Completed)";
        }
        else if (!Commission.IsCompleted)
        {
            _MarkedCompleted = false;
            _Text.text = Commission.Title;
        }
    }
}
