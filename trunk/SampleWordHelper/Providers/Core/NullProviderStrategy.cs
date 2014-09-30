using System;
using SampleWordHelper.Core;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Реализация стратегии провайдера, не выполняющая никаких действий.
    /// </summary>
    public class NullProviderStrategy : IProviderStrategy, ICatalogStrategy, IConfigurationManager
    {
        /// <summary>
        /// Экземпляр стратегии.
        /// </summary>
        public static readonly NullProviderStrategy INSTANCE = new NullProviderStrategy();

        /// <summary>
        /// Singleton-конструктор.
        /// </summary>
        NullProviderStrategy()
        {
        }

        public IConfigurationManager ConfigurationManager
        {
            get { return this; }
        }

        public ICatalogStrategy CatalogStrategy
        {
            get { return this; }
        }

        public bool Initialize(IRuntimeContext context, bool reinitialize)
        {
            return false;
        }

        public void Shutdown()
        {
        }

        public CatalogModel LoadCatalog(CatalogLoadMode mode)
        {
            throw new InvalidOperationException();
        }

        public ISettingsModel CreateSettingsModel()
        {
            throw new InvalidOperationException();
        }

        public void ApplyConfiguration(ISettingsModel model)
        {
        }
    }
}