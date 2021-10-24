public interface IHarvest
{
    public void Initialize(FloraInfo data, IHarvest harvestData);
    public bool Harvest(Flora flora, int playerIndex);
}
