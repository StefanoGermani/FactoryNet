using System;
using System.Collections.Generic;
using FactoryNet.Core.Helpers;

namespace FactoryNet.Core.Dictionaries
{
    internal interface IPostCreationActionDictionary
    {
        void Add<T>(string variation, Action<T> afterCreation);
        bool ContainsKey<T>(string variation);
        void ExecuteAction<T>(string variation, T constructedObject);
    }

    internal class PostCreationActionDictionary : BaseList<object>, IPostCreationActionDictionary
    {
        public void Add<T>(string variation, Action<T> afterCreation)
        {
            base.Add<T>(variation, afterCreation);
        }

        public void ExecuteAction<T>(string variation, T constructedObject)
        {
            ((Action<T>)base.Get<T>(variation))(constructedObject);
        }
    }
}