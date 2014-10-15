using System;
using Microsoft.Office.Interop.Word;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Presentation;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Состояние менеджера главного представления.
    /// </summary>
    public class MainPresenterState: BasicDisposable
    {
        /// <summary>
        /// Слушатель событий окружения.
        /// </summary>
        readonly ApplicationEventsListener eventListener;

        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        readonly ConfigurationModel configuration;
        
        /// <summary>
        /// Контекст выполнения приложения.
        /// </summary>
        readonly ApplicationContext context;

        /// <summary>
        /// Объект для отслеживания состояния документов.
        /// </summary>
        readonly DocumentManager documentManager;

        /// <summary>
        /// Экземпляр активного поставщика каталога.
        /// </summary>
        Provider Provider { get; set; }

        /// <summary>
        /// Флаг, равный true, если состояние корректно проинициализировано.
        /// </summary>
        public bool IsValid { get; private set; }

        /// <summary>
        /// Сообщение, описывающее ошибку, если она имело место.
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Создаёт новый экземпляр состояния.
        /// </summary>
        /// <param name="context">Контекст приложения.</param>
        /// <param name="callback">Объект для выполнения обратных вызовов от панели каталога.</param>
        public MainPresenterState(ApplicationContext context, ICatalogPaneCallback callback)
        {
            this.context = context;
            documentManager = new DocumentManager(context, callback);
            eventListener = new ApplicationEventsListener(context.Environment, documentManager);
            configuration = new ConfigurationModel("reportHelper");
            Provider = new Provider(new NullProviderStrategy());
            InitializeAndValidate();
            eventListener.StartListen();
        }

        /// <summary>
        /// Проверяет состояние провайдера.
        /// Если он успешно инициализировался, делает доступными все элементы управления надстройкой, в противном случае требует повторной настройки.
        /// </summary>
        bool InitializeAndValidate()
        {
            if (!configuration.HasConfiguredProvider)
                return SetInvalid("Требуется выбор поставщика каталога.");
            Provider = new Provider(configuration.GetConfiguredProviderStrategy());
            if (!Provider.Initialize(context.Environment))
                return SetInvalid("Требуется настройка текущего поставщика каталога.");
            Message = null;
            return IsValid = true;
        }

        /// <summary>
        /// Создаёт модель для редактирования конфигурации.
        /// </summary>
        public ConfigurationEditorModel CreateEditorModel()
        {
            return configuration.CreateEditorModel();
        }

        /// <summary>
        /// Обновляет конфигурацию приложения.
        /// </summary>
        /// <param name="model">Модель для редактирования, на основании которой обновляется конфигурация.</param>
        public void UpdateConfiguraion(ConfigurationEditorModel model)
        {
            Provider.Shutdown();
            configuration.Update(model);
            Provider = new Provider(configuration.GetConfiguredProviderStrategy());
            InitializeAndValidate();
        }

        /// <summary>
        /// Выполняет обновление каталога.
        /// </summary>
        public void UpdateCatalog()
        {
            context.Catalog = Provider.LoadCatalog();
            documentManager.UpdateCatalog();
        }

        /// <summary>
        /// Приостанавливает обработку событий приложения.
        /// </summary>
        public IDisposable SuspendUpdates()
        {
            return new SuspendedState(documentManager, eventListener.SuspendEvents());
        }

        /// <summary>
        /// Устанавливает видимость панели каталога в зависимости от флага.
        /// </summary>
        /// <param name="visible">True, если каталог видим, иначе false.</param>
        public void ShowCatalogPane(bool visible)
        {
            documentManager.UpdateCatalogVisibility(IsValid && visible);
        }

        /// <summary>
        /// Устанавливает ошибку валидации и сообщение.
        /// </summary>
        bool SetInvalid(string message)
        {
            IsValid = false;
            Message = message;
            return false;
        }

        protected override void DisposeManaged()
        {
            eventListener.SafeDispose();
            documentManager.SafeDispose();
            Provider.Shutdown();
        }

        /// <summary>
        /// Приостановленное состояние.
        /// </summary>
        class SuspendedState : IDisposable
        {
            readonly IDisposable events;
            readonly DocumentManager man;

            public SuspendedState(DocumentManager man, IDisposable events)
            {
                this.man = man;
                this.events = events;
                man.UpdateCatalogVisibility(false);
            }

            public void Dispose()
            {
                events.Dispose();
                man.UpdateCatalogVisibility(true);
            }
        }
    }
}