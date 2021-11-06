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
        _Text.color = _NormalColor + new Color(-0.3f, -0.3f, -0.3f, 1.0f);
        CommissionLog.Instance.ShowDescription(Commission);
    }

    public void Deselect()
    {
        _Text.color = (Commission.IsCompleted) ? _NormalColor + new Color(-0.2f, 0.5f, -0.2f, 1.0f) : _NormalColor;
    }

    public void IsCompleted()
    {
        _Text.color = (Commission.IsCompleted) ? _NormalColor + new Color(-0.2f, 0.5f, -0.2f, 1.0f) : _NormalColor;
    }
}
