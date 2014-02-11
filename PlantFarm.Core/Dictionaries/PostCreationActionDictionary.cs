using System;
using System.Collections.Generic;
using PlantFarm.Core.Helpers;

namespace PlantFarm.Core.Dictionaries
{
    internal class PostCreationActionDictionary
    {
        private readonly IBluePrintKeyHelper _bluePrintKeyHelper;

        private readonly IDictionary<string, object> _postCreationActions = new Dictionary<string, object>();

        public PostCreationActionDictionary(IBluePrintKeyHelper bluePrintKeyHelper)
        {
            _bluePrintKeyHelper = bluePrintKeyHelper;
        }

        public void Add<T>(string variation, Action<T> afterCreation)
        {
            _postCreationActions.Add(_bluePrintKeyHelper.GetBluePrintKey<T>(variation), afterCreation);
        }

        public bool ContainsKey<T>(string variation)
        {
            return _postCreationActions.ContainsKey(_bluePrintKeyHelper.GetBluePrintKey<T>(variation));
        }

        public void ExecuteAction<T>(string variation, T constructedObject)
        {
            ((Action<T>)_postCreationActions[_bluePrintKeyHelper.GetBluePrintKey<T>(variation)])(constructedObject);
        }
    }
}