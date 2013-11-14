using System;
using System.Linq.Expressions;
using Plant.Core.Impl;

namespace Plant.Core
{
    public interface IPlant
    {
        event BluePrintCreatedEventHandler BluePrintCreated;
        T CreateForChild<T>();
        T Build<T>();
        //T Build<T>(string variation);
        T Build<T>(Action<T> userSpecifiedProperties);
        T Create<T>();
        //T Create<T>(string variation);
        T Create<T>(Action<T> userSpecifiedProperties);
        //T Create<T>(Action<T> userSpecifiedProperties, string variation, bool created);
       
        void Define<T>(Expression<Func<T>> definition);
        void Define<T>(Expression<Func<T>> definition, Action<T> afterCreation);
    }
}