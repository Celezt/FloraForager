public interface IHarvest : IUsable
{
    public void Initialize(FloraData data, IHarvest harvestData);
    public void Harvest(Flora flora, int playerIndex);
}
