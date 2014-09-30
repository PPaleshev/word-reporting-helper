using System;
using System.Threading;
using SampleWordHelper.Core;
using SampleWordHelper.Model;

namespace SampleWordHelper.Providers.Core
{
    /// <summary>
    /// Класс, обеспечивающий контроль жизненного цикла провайдера.
    /// Спроектирован с учётом потокобезопасности.
    /// По умолчанию находится в неактивном состоянии.
    /// </summary>
    public struct Provider 
    {
        /// <summary>
        /// Стратегия провайдера каталога.
        /// </summary>
        readonly IProviderStrategy strategy;

        /// <summary>
        /// Флаг, равный true, если текущий провайдер активен, иначе false.
        /// </summary>
        int isActive;

        /// <summary>
        /// Создаёт новый экземпляр провайдера.
        /// </summary>
        public Provider(IProviderStrategy strategy)
        {
            if (strategy == null)
                throw new ArgumentNullException("strategy");
            this.strategy = strategy;
            isActive = 0;
        }

        /// <summary>
        /// Выполняет инициализацию провайдера.
        /// </summary>
        /// <param name="context">Контекст времени исполнения.</param>
        /// <returns>Возвращает true, если настройки успешно загружены и провайдер успешно проинициализирован, и false, если требуется настройка пользователем.</returns>
        public bool Initialize(IRuntimeContext context)
        {
            var initialized = Thread.VolatileRead(ref isActive) == 1;
            var result = strategy.Initialize(context, initialized);
            Thread.VolatileWrite(ref isActive, result ? 1 : 0);
            return result;
        }

        /// <summary>
        /// Вызывается для завершения работы с каталогом.
        /// </summary>
        public void Shutdown()
        {
            if (Thread.VolatileRead(ref isActive) == 0)
                throw new InvalidOperationException("failed to shutdown provider due to inactive state");
            try
            {
                strategy.Shutdown();
            }
            finally
            {
                Thread.VolatileWrite(ref isActive, 0);
            }
        }

        /// <summary>
        /// Сохраняет конфигурацию провайдера.
        /// </summary>
        /// <param name="model">Модель конфигурации.</param>
        public void ApplyConfiguration(ISettingsModel model)
        {
            if (Thread.VolatileRead(ref isActive) == 0)
                throw new InvalidOperationException("cannot apply configuration due to inactive provider state");
            strategy.ConfigurationManager.ApplyConfiguration(model);
        }

        /// <summary>
        /// Загружает данные каталога.
        /// </summary>
        /// <param name="mode">Режим загрузки каталога.</param>
        public CatalogModel LoadCatalog(CatalogLoadMode mode)
        {
            if (Thread.VolatileRead(ref isActive) == 0)
                throw new InvalidOperationException("cannot perform catalog loading due to inactive state");
            return strategy.CatalogStrategy.LoadCatalog(mode);
        }
    }
}
