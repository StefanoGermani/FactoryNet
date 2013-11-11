using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Plant.Core.Exceptions;

namespace Plant.Core
{
    public class PersisterPlant : BasePlant
    {
        private readonly IPersisterSeed _persisterSeed;

        public PersisterPlant(IPersisterSeed persisterSeed)
        {
            if (persisterSeed == null)
            {
                throw new PersisterException("Persister is null");
            }

            _persisterSeed = persisterSeed;
        }

        public override T Create<T>(object userSpecifiedProperties, string variation, bool created)
        {
            var constructedObject = base.Create<T>(userSpecifiedProperties, variation, true);

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
            }


            return constructedObject;
        }
    }
}
