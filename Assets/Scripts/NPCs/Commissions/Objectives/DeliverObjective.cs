using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[System.Serializable]
public class DeliverObjective : IObjective
{
    public bool IsCompleted => false;
    public string Status => string.Empty;

    public void Initialize(IObjective objectiveData)
    {

    }

    public void Accepted()
    {

    }
    public void Complete()
    {

    }
    public void Removed()
    {

    }

    public void UpdateStatus()
    {

    }
}
