namespace FactoryNet.Core
{
    public interface IPersister
    {
        bool Save(object dto);
        bool Delete(object dto);
    }
}