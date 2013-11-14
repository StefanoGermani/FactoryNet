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


        protected override void OnBluePrintCreated(BluePrintEventArgs e)
        {
             try
             {
                 if (!_persisterSeed.Save(e.ObjectConstructed))
                 {
                     throw new PersisterException();
                 }

             }
             catch (Exception ex)
             {
                 throw new PersisterException(ex);
             }

            base.OnBluePrintCreated(e);
        }
    }
}
