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

    internal class PostCreationActionDictionary : IPostCreationActionDictionary
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