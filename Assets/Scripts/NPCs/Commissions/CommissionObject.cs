using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CommissionObject : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _Text;

    private Image _Background;
    private Color _NormalColor;

    public Image Background => _Background;
    public TMP_Text Text => _Text;

    public Commission Commission { get; set; }

    private void Awake()
    {
        _Background = GetComponent<Image>();
        _NormalColor = _Text.color;
    }

    public void Select()
    {
        _Text.color = _NormalColor + new Color(-0.3f, -0.3f, -0.3f, 1.0f);
        CommissionLog.Instance.ShowDescription(Commission);
    }

    public void Deselect()
    {
        _Text.color = (Commission.IsCompleted) ? _NormalColor + new Color(-0.3f, 0.5f, -0.3f, 1.0f) : _NormalColor;
    }

    public void IsCompleted()
    {
        _Text.color = (Commission.IsCompleted) ? _NormalColor + new Color(-0.3f, 0.5f, -0.3f, 1.0f) : _NormalColor;
    }
}
