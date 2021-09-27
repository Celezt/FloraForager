using UnityEngine;

public class CGCommissionObject : MonoBehaviour
{
    public Commission Commission { get; set; }

    public void Select()
    {
        CommissionGiverWindow.Instance.ShowCommissionInfo(Commission);
    }
}
