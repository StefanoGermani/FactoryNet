using System;
using Plant.Core.Exceptions;

namespace Plant.Core.Impl
{
    internal class PersisterPlant : BasePlant
    {
        //private readonly IDictionary<Type, object> _postCreationActions = new Dictionary<Type, object>();
        //private readonly IDictionary<string, object> _postCreationVariationActions = new Dictionary<string, object>();

        private readonly IPersisterSeed _persisterSeed;

        public PersisterPlant(IPersisterSeed persisterSeed)
        {
            if (persisterSeed == null)
            {
                throw new PersisterException("Persister is null");
            }

            _persisterSeed = persisterSeed;
        }

        public override T Create<T>(Action<T> userSpecifiedProperties, string variation, bool created)
        {
            var constructedObject = base.Create<T>(userSpecifiedProperties, variation, false);

            if (created)
            {
                try
                {
                    if (!_persisterSeed.Save(constructedObject))
                    {
                        throw new PersisterException();
                    }

                }
                catch (Exception ex)
                {
                    throw new PersisterException(ex);
                }


                //string bluePrintKey = BluePrintKey<T>(variation);

                //if (_postCreationActions.ContainsKey(typeof(T)))
                //    ((Action<T>)_postCreationActions[typeof(T)])(constructedObject);

                //if (_postCreationVariationActions.ContainsKey(bluePrintKey))
                //    ((Action<T>)_postCreationVariationActions[bluePrintKey])(constructedObject);
            }

            return constructedObject;
        }

        //public override void DefinePropertiesOf<T>(T defaults, Action<T> afterCreation)
        //{
        //    base.DefinePropertiesOf(defaults);

        //    _postCreationActions[typeof(T)] = afterCreation;
        //}

        //public override void DefineVariationOf<T>(string variation, object defaults, Action<T> afterCreation)
        //{
        //    base.DefineVariationOf<T>(variation, defaults);

        //    _postCreationVariationActions[BluePrintKey<T>(variation)] = afterCreation;

        //}
    }
}
