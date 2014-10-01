using System;
using SampleWordHelper.Core;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    public class NullProviderStrategy : ICatalogProviderStrategy
    {
        public bool Initialize(IRuntimeContext context)
        {
            return false;
        }

        public ICatalog LoadCatalog()
        {
            throw new NotSupportedException();
        }

        public void Shutdown()
        {
            throw new NotSupportedException();
        }
    }
}