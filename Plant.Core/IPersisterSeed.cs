namespace Plant.Core
{
    public interface IPersisterSeed
    {
        bool Save<T>(T dto);
    }
}