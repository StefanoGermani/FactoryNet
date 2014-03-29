using System;
using FactoryNet.Core.Exceptions;

namespace FactoryNet.Core
{
    public class PersisterFactory : BaseFactory
    {
        private readonly IPersister _persister;

        public PersisterFactory(IPersister persister)
        {
            if (persister == null)
            {
                throw new PersisterException("Persister is null");
            }

            _persister = persister;
        }

        protected override void OnBluePrintCreated(ObjectEventArgs e)
        {
            try
            {
                if (!_persister.Save(e.Object))
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
                if (!_persister.Delete(e.Object))
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