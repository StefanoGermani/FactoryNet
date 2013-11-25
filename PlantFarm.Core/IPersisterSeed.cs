namespace PlantFarm.Core
{
    public interface IPersisterSeed
    {
        bool Save(object dto);
        bool Delete(object dto);
    }
}