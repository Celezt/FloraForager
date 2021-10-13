using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IHarvest : IUsable
{
    public void Initialize(FloraData data);
    public void Harvest(Flora flora, int playerIndex);
}
