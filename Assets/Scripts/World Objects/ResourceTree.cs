using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

class ResourceTree : ResourceSource
{
    private float _ChopTimeLeft;

    public override float Strength { get; set; }
    public override float Durability { get; set; }

    protected override void Start()
    {
        base.Start();

        _ChopTimeLeft = Data.TotalCollectionTime;
    }

    protected override bool StartCollecting()
    {
        bool result = false;

        if (!(result = base.StartCollecting()))
            return result;



        return result;
    }

    protected override bool StopCollecting()
    {
        bool result = false;

        if (!(result = base.StopCollecting()))
            return result;



        return result;
    }

    protected override IEnumerator Collect()
    {
        while (true)
        {
            _ChopTimeLeft -= Time.deltaTime;

            if (_ChopTimeLeft < 0.0f)
            {
                StopCollecting();
                OnEmptied.Invoke();

                _Inventory.Insert(new ItemAsset
                {
                    ID = Data.ItemID,
                    Amount = Data.Amount
                });

                Destroy(gameObject);  // simply destroy for now 
                ResourceSourceUI.Instance.SetActive(null, false);

                break;
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public override ItemLabels Filter() => ItemLabels.Axe;
}
