using System.Collections.Generic;
using FactoryNet.Core.Helpers;

namespace FactoryNet.Core.Dictionaries
{
    internal interface ICreatedBlueprints
    {
        void Add<T>(string variation, T createdObject);
        bool ContainsKey<T>(string variation);
        T Get<T>(string varation);
    }

    internal class CreatedBlueprints : BaseObject<object>, ICreatedBlueprints
    {
        public void Add<T>(string variation, T createdObject)
        {
            base.Add<T>(variation, createdObject);
        }

        public new T Get<T>(string varation)
        {
            return (T)base.Get<T>(varation);
        }
    }
}