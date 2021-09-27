using UnityEngine;
using UnityEngine.UI;

public class CommissionObject : MonoBehaviour
{
    private Text _Text;
    private Color _NormalColor;

    public Commission Commission { get; set; }

    private void Awake()
    {
        _Text = GetComponent<Text>();
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
