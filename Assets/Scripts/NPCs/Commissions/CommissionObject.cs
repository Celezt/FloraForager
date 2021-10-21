using UnityEngine;
using TMPro;

public class CommissionObject : MonoBehaviour
{
    private TMP_Text _Text;
    private Color _NormalColor;

    public Commission Commission { get; set; }

    private void Awake()
    {
        _Text = GetComponent<TMP_Text>();
        _NormalColor = _Text.color;
    }

    public void Select()
    {
        _Text.color = Color.red;
        CommissionLog.Instance.ShowDescription(Commission);
    }

    public void Deselect()
    {
        _Text.color = (Commission.IsCompleted) ? Color.green : _NormalColor;
    }

    public void IsCompleted()
    {
        _Text.color = (Commission.IsCompleted) ? Color.green : _NormalColor;
    }
}
