using System;
using System.Collections.Generic;

namespace PlantFarm.Core.Dictionaries
{
    internal class PostCreationActionDictionary : BaseDictionary
    {
        private readonly IDictionary<string, object> _postCreationActions = new Dictionary<string, object>();

        public void Add<T>(string variation, Action<T> afterCreation)
        {
            _postCreationActions.Add(BluePrintKey<T>(variation), afterCreation);
        }

        public bool ContainsKey<T>(string variation)
        {
            return _postCreationActions.ContainsKey(BluePrintKey<T>(variation));
        }

        public void ExecuteAction<T>(string variation, T constructedObject)
        {
            ((Action<T>)_postCreationActions[BluePrintKey<T>(variation)])(constructedObject);
        }
    }
}