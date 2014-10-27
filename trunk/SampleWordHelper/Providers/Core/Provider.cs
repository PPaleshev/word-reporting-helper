using System;
using System.Threading;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Класс, предоставляющий доступ к реализации провайдера.
    /// </summary>
    public class Provider
    {
        /// <summary>
        /// Флаг, равный 1, если текущий провайдер активен в данный момент, иначе false.
        /// </summary>
        int isActive;

        /// <summary>
        /// Экземпляр стратегии, которой делегируются все методы.
        /// </summary>
        readonly ICatalogProviderStrategy strategy;

        /// <summary>
        /// Возвращает true, если провайдер может быть использован для работы, иначе false.
        /// </summary>
        public bool IsActive { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр провайдера.
        /// </summary>
        public Provider(ICatalogProviderStrategy strategy)
        {
            this.strategy = strategy;
            IsActive = !(strategy is NullProviderStrategy);
        }

        /// <summary>
        /// Выполняет инициализацию провайдера.
        /// </summary>
        /// <param name="context">Контекст времени исполнения.</param>
        /// <returns>Возвращает true, если провайдер успешно проинициализирован, иначе false.</returns>
        public bool Initialize(IRuntimeContext context)
        {
            var result = strategy.Initialize(context);
            Thread.VolatileWrite(ref isActive, result ? 1 : 0);
            return result;
        }

        /// <summary>
        /// Строит модель каталога.
        /// </summary>
        public CatalogLoadResult LoadCatalog()
        {
            if (Thread.VolatileRead(ref isActive) != 1)
                throw new InvalidOperationException("failed to load catalog due to inactive state");
            try
            {
                return strategy.LoadCatalog();
            }
            catch (Exception e)
            {
                throw new CatalogLoadException(e.Message, e);
            }
        }

        /// <summary>
        /// Завершает активность текущего провайдера.
        /// </summary>
        public void Shutdown()
        {
            if (Thread.VolatileRead(ref isActive) != 1)
                return;
            try
            {
                strategy.Shutdown();
            }
            finally
            {
                Thread.VolatileWrite(ref isActive, 0);
            }
        }
    }
}