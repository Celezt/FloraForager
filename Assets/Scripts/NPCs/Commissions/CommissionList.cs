using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using MyBox;
using Sirenix.Serialization;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Commission List", menuName = "Game Logic/Commission List")]
[System.Serializable]
public class CommissionList : SerializedScriptableSingleton<CommissionList>, IStreamer
{
    [SerializeField]
    private System.Guid _Guid;

    [System.NonSerialized]
    private List<Commission> _Commissions = new List<Commission>();

    public List<Commission> Commissions => _Commissions;

    private void Awake()
    {
        if (_Guid == System.Guid.Empty)
            _Guid = System.Guid.NewGuid();

        GameManager.AddStreamer(this);
    }

#if UNITY_EDITOR
    [UnityEditor.InitializeOnEnterPlayMode]
    private static void PlayModeInitialize()
    {
        GameManager.AddStreamer(Instance);
    }
#endif

    public void Add(Commission commission)
    {
        _Commissions.Add(commission);
    }

    public bool Remove(Commission commission)
    {
        if (commission == null)
            return false;

        return _Commissions.Remove(commission);
    }

    public void Notify()
    {
        for (int i = _Commissions.Count - 1; i >= 0; --i)
        {
            _Commissions[i].DayPassed();
        }
    }

    public bool HasCommission(Commission commission)
    {
        return _Commissions.Exists(c => 
            c.CommissionData.Title == commission.CommissionData.Title && 
            c.CommissionData.Giver == commission.CommissionData.Giver);
    }

    public void UpLoad()
    {
        Dictionary<string, object> streamables = new Dictionary<string, object>();

        _Commissions.ForEach(x => streamables.Add(x.Title + ", " + x.Giver, x.OnUpload()));

        GameManager.Stream.Load(_Guid, streamables);
    }
    public void Load()
    {
        _Commissions.Clear();

        if (!GameManager.Stream.TryGet(_Guid, out Dictionary<string, object> streamables))
            return;

        foreach (KeyValuePair<string, object> item in streamables)
        {
            if (!streamables.TryGetValue(item.Key, out object value))
                continue;

            Commission.Data data = value as Commission.Data;

            Commission commission = new Commission();
            commission.OnLoad(data);

            _Commissions.Add(commission);
        }
    }
    public void BeforeSaving()
    {
        GameManager.Stream.Release(_Guid);

        UpLoad(); // reupload to update to latest changes

        _Commissions.ForEach(x => x.OnBeforeSaving());
    }
}
