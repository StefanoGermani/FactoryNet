using System;

namespace Plant.Core
{
    public interface IPlant
    {
        event BluePrintCreatedEventHandler BluePrintCreated;
        T CreateForChild<T>();
        T Build<T>();
        T Build<T>(string variation);
        T Build<T>(T userSpecifiedProperties);
        T Create<T>();
        T Create<T>(string variation);
        T Create<T>(T userSpecifiedProperties);
        T Create<T>(object userSpecifiedProperties);
        T Create<T>(object userSpecifiedProperties, string variation, bool created);
        void DefinePropertiesOf<T>(T defaults);
        void DefinePropertiesOf<T>(T defaults, Action<T> afterPropertyPopulation);
        void DefinePropertiesOf<T>(object defaults);
        void DefineVariationOf<T>(string variation, T defaults);
        void DefineVariationOf<T>(string variation, T defaults, Action<T> afterPropertyPopulation);
        void DefineVariationOf<T>(string variation, object defaults);
        void DefineVariationOf<T>(string variation, object defaults, Action<T> afterPropertyPopulation);
        void DefineConstructionOf<T>(object defaults, Action<T> afterCtorPopulation);
        void DefineConstructionOf<T>(object defaults);
    }
}