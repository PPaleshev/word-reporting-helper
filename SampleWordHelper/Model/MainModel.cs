using System;
using SampleWordHelper.Core.Application;
using SampleWordHelper.Core.Common;
using SampleWordHelper.Presentation;
using SampleWordHelper.Providers.Core;

namespace SampleWordHelper.Model
{
    /// <summary>
    /// Состояние менеджера главного представления.
    /// </summary>
    public class MainModel : BasicDisposable
    {
        /// <summary>
        /// Конфигурация приложения.
        /// </summary>
        readonly ConfigurationModel configuration;
        
        /// <summary>
        /// Контекст выполнения приложения.
        /// </summary>
        readonly ApplicationContext context;

        /// <summary>
        /// Объект для выполнения обратных вызовов от панели каталога.
        /// </summary>
        readonly ICatalogPaneCallback callback;

        /// <summary>
        /// Активное состояние.
        /// </summary>
        ActiveState state;

        /// <summary>
        /// Флаг, равный true, если модель активна, иначе false.
        /// </summary>
        public bool IsActive { get; private set; }

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
        public MainModel(ApplicationContext context, ICatalogPaneCallback callback)
        {
            this.context = context;
            this.callback = callback;
            configuration = new ConfigurationModel("reportHelper");
            IsActive = false;
            IsValid = false;
            Message = "Приложение не выполняется.";
        }

        /// <summary>
        /// Активирует модель.
        /// </summary>
        public void Activate()
        {
            if (IsActive)
                throw new InvalidOperationException("model is already active");
            IsActive = true;
            state = new ActiveState(context, callback);
            InitializeAndValidate();
        }

        /// <summary>
        /// Завершает активность.
        /// </summary>
        public void Shutdown()
        {
            if (!IsActive)
                throw new InvalidOperationException("model is inactive");
            IsActive = false;
            state.SafeDispose();
            state = null;
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
            if (!IsActive)
            {
                configuration.Update(model);
                return;
            }
            state.Provider.Shutdown();
            configuration.Update(model);
            state.Provider = new Provider(configuration.GetConfiguredProviderStrategy());
            InitializeAndValidate();
        }

        /// <summary>
        /// Выполняет обновление каталога.
        /// </summary>
        public void UpdateCatalog()
        {
            context.Catalog = state.Provider.LoadCatalog();
            state.documentManager.UpdateCatalog();
        }

        /// <summary>
        /// Приостанавливает обработку событий приложения.
        /// </summary>
        public IDisposable SuspendUpdates()
        {
            return state.SuspendUpdates();
        }

        /// <summary>
        /// Устанавливает видимость панели каталога в зависимости от флага.
        /// </summary>
        /// <param name="visible">True, если каталог видим, иначе false.</param>
        public void ShowCatalogPane(bool visible)
        {
            state.documentManager.UpdateCatalogVisibility(IsValid && visible);
        }

        /// <summary>
        /// Проверяет состояние провайдера.
        /// Если он успешно инициализировался, делает доступными все элементы управления надстройкой, в противном случае требует повторной настройки.
        /// </summary>
        bool InitializeAndValidate()
        {
            if (!configuration.HasConfiguredProvider)
                return SetInvalid("Требуется выбор поставщика каталога.");
            state.Provider = new Provider(configuration.GetConfiguredProviderStrategy());
            if (!state.Provider.Initialize(context.Environment))
                return SetInvalid("Требуется настройка текущего поставщика каталога.");
            Message = null;
            return IsValid = true;
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
            if (state != null)
                state.SafeDispose();
        }

        sealed class ActiveState : BasicDisposable
        {
            /// <summary>
            /// Объект для отслеживания состояния документов.
            /// </summary>
            public readonly DocumentManager documentManager;

            /// <summary>
            /// Слушатель событий окружения.
            /// </summary>
            public readonly ApplicationEventsListener eventListener;

            /// <summary>
            /// Экземпляр активного поставщика каталога.
            /// </summary>
            public Provider Provider { get; set; }

            public ActiveState(ApplicationContext context, ICatalogPaneCallback callback)
            {
                documentManager = new DocumentManager(context, callback);
                eventListener = new ApplicationEventsListener(context.Environment, documentManager);
                Provider = new Provider(new NullProviderStrategy());
                eventListener.StartListen();
            }

            /// <summary>
            /// Приостанавливает обработку событий приложения.
            /// </summary>
            public IDisposable SuspendUpdates()
            {
                return new SuspendedEvents(documentManager, eventListener.SuspendEvents());
            }

            protected override void DisposeManaged()
            {
                eventListener.SafeDispose();
                documentManager.SafeDispose();
                if (Provider != null)
                    Provider.Shutdown();
            }

            /// <summary>
            /// Приостановленное состояние.
            /// </summary>
            class SuspendedEvents : IDisposable
            {
                readonly IDisposable events;
                readonly DocumentManager man;

                public SuspendedEvents(DocumentManager man, IDisposable events)
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
}