using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

class ResourceStone : ResourceSource
{
    public override float Strength { get; set; }
    public override float Durability { get; set; }

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
        yield return base.Collect();
    }

    public override ItemLabels Filter() => ItemLabels.Pickaxe;
}
