using System;
using PlantFarm.Core.Exceptions;

namespace PlantFarm.Core.Impl
{
    internal class PersisterPlant : BasePlant
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

        protected override void OnBluePrintCreated(ObjectEventArgs e)
        {
            try
            {
                if (!_persisterSeed.Save(e.Object))
                {
                    throw new PersisterException();
                }
            }
            catch (PersisterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PersisterException(ex);
            }

            base.OnBluePrintCreated(e);
        }

        protected override void OnObjectDeleted(ObjectEventArgs e)
        {
            try
            {
                if (!_persisterSeed.Delete(e.Object))
                {
                    throw new PersisterException();
                }
            }
            catch (PersisterException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new PersisterException(ex);
            }

            base.OnObjectDeleted(e);
        }
    }
}